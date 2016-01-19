using Aton.AtonSocket.Core;
using Aton.AtonSocket.Core.EventArgs;
using Aton.AtonSocket.Core.Exceptions;
using Aton.AtonSocket.Core.Filter;
using Aton.AtonSocket.Core.Handler;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Aton.AtonSocket
{
    internal class UdpSocketAsyncSession : SocketSessionBase
    {
        #region Session Param

        ISocketServer m_SocketServer;

        Socket m_ClientSocket;

        SocketAsyncEventArgs m_SocketAsyncEventArgs;

        ConcurrentDictionary<string, UdpSocketAsyncBuffer> m_socketBuffer;

        UdpSocketAsyncSender m_SocketAsyncSender;

        int SessionTimeout;

        #endregion

        #region Session Event
        /// <summary>
        /// Session Close Event
        /// </summary>
        public event EventHandler<SessionCloseEventArgs> SessionClosed;
        /// <summary>
        /// New Client Connected Event
        /// </summary>
        public event EventHandler<SessionConnectedEventArgs> SessionConnected;
        #endregion

        #region Session Process
        /// <summary>
        /// Construction 
        /// </summary>
        /// <param name="socket">client socket</param>
        /// <param name="socketServer">the socket server</param>
        /// <param name="logger">logger object for ouput session message</param>
        internal UdpSocketAsyncSession(Socket socket, ISocketServer socketServer, IMsgProtocol protocol, IList<IMsgFilter> requestFilters, IList<IMsgHandler> requestHandlers, ILogger logger)
            : base(protocol, requestFilters, requestHandlers, logger)
        {
            SessionTimeout = (socketServer as UdpSocketAsyncServer).SessionTimeout;
            m_ClientSocket = socket;
            m_SocketServer = socketServer;
            m_Logger = logger;
            m_SocketAsyncSender = new UdpSocketAsyncSender(socket, socketServer);
        }

        /// <summary>
        /// start session running in new task
        /// </summary>
        /// <param name="socketEventArgRecv">SAEA for receive </param>
        internal void StartSession(SocketAsyncEventArgs saea, ConcurrentDictionary<string, UdpSocketAsyncBuffer> buffer)
        {
            m_socketBuffer = buffer;

            m_SocketAsyncEventArgs = saea;
            m_SocketAsyncEventArgs.Completed += Saea_Completed;

            IPEndPoint ipe = saea.RemoteEndPoint as IPEndPoint;
            string sessionID = string.Format("{0}:{1}", ipe.Address.ToString(), ipe.Port);

            SocketAsyncToken token = saea.UserToken as SocketAsyncToken;
            buffer[sessionID].SocketToken.BufferSize = token.BufferSize;
            buffer[sessionID].SocketToken.LastLength = token.LastLength;
            buffer[sessionID].SocketToken.OriginOffset = 0;
            buffer[sessionID].SocketToken.OffsetDelta = token.OffsetDelta - token.OriginOffset;

            if (SessionConnected != null)
            {
                var remote = saea.RemoteEndPoint as IPEndPoint;
                SessionConnected(this, new SessionConnectedEventArgs((IPEndPoint)remote));
            }
            StartReceive(saea);
        }



        private void Saea_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.ReceiveFrom:
                    ProcessReceive(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive");
            }
        }

        public void StartReceive(SocketAsyncEventArgs e, UdpSocketAsyncSender asyncSender = null, ConcurrentDictionary<string, UdpSocketAsyncBuffer> buffer = null)
        {
            bool isPeeding = m_ClientSocket.ReceiveFromAsync(e);
            if (!isPeeding)
            {
                ProcessReceive(e);
            }
        }

        /// <summary>
        /// process received message
        /// </summary>
        /// <param name="e">saea object</param>
        public void ProcessReceive(SocketAsyncEventArgs e)
        {
            IPEndPoint ipe = e.RemoteEndPoint as IPEndPoint;
            string sessionID = string.Format("{0}:{1}", ipe.Address.ToString(), ipe.Port);

            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success && m_ClientSocket != null)
            {
                try
                {
                    SocketAsyncToken token = e.UserToken as SocketAsyncToken;
                    IPEndPoint ip = new IPEndPoint(((IPEndPoint)e.RemoteEndPoint).Address, ((IPEndPoint)e.RemoteEndPoint).Port);

                    if (m_socketBuffer.ContainsKey(sessionID))
                    {
                        UdpSocketAsyncBuffer buf = m_socketBuffer[sessionID];
                        lock (buf)
                        {
                            Buffer.BlockCopy(e.Buffer, token.OriginOffset, buf.buffer, buf.SocketToken.OffsetDelta, e.BytesTransferred);
                            m_socketBuffer[sessionID].SocketToken = ReceiveMessageProcess(m_socketBuffer[sessionID].buffer, e.BytesTransferred, m_socketBuffer[sessionID].SocketToken, ip);
                        }
                        m_socketBuffer[sessionID].LastSocketTime = DateTime.Now;
                    }
                    else
                    {
                        string response = string.Format("Error:Session Timeout or Not Register");
                        SendSync(ASCIIEncoding.Default.GetBytes(response), e.RemoteEndPoint as IPEndPoint);
                    }


                    //自检：检查当前SocketAsyncEventArgs关联创建者Session的ID时候过期了
                    if (m_socketBuffer.ContainsKey(token.TokenID))
                    {
                        //至少保留一个线程
                        if (m_socketBuffer.Count > 1 && DateTime.Now - m_socketBuffer[token.TokenID].LastSocketTime > new TimeSpan(0, 0, SessionTimeout))
                        {
                            CloseSession("GC SocketAsyncEventArgs Object", e);
                            return;
                        }
                    }
                }
                catch (ProtocolException ex)
                {
                    m_Logger.ErrorLogger(ex.Message, ex.StackTrace);
                    if (ex.InnerException != null)
                    {
                        m_Logger.ErrorLogger("-----exception detail----", "");
                        m_Logger.ErrorLogger(ex.InnerException.Message, ex.InnerException.StackTrace);
                    }
                }
                catch (MsgHandlerException ex)
                {
                    m_Logger.ErrorLogger(ex.Message, ex.StackTrace);
                    if (ex.InnerException != null)
                    {
                        m_Logger.ErrorLogger("-----exception detail----", "");
                        m_Logger.ErrorLogger(ex.InnerException.Message, ex.InnerException.StackTrace);
                    }
                }
                catch (Exception ex)
                {
                    m_Logger.ErrorLogger(ex.Message, ex.StackTrace);
                    if (ex.InnerException != null)
                    {
                        m_Logger.ErrorLogger("-----exception detail----", "");
                        m_Logger.ErrorLogger(ex.InnerException.Message, ex.InnerException.StackTrace);
                    }

                }
                StartReceive(e);
            }
        }

        /// <summary>
        ///  use protocol and handler to process received message 
        /// </summary>
        /// <param name="buffer">message buffer</param>
        /// <param name="bytesTransferred">received message length</param>
        /// <param name="token">user token</param>
        /// <returns>user token for next receive </returns>
        private SocketAsyncToken ReceiveMessageProcess(byte[] buffer, int bytesTransferred, SocketAsyncToken token, IPEndPoint remoteEndPoint)
        {
            IMsg request;
            token.LastLength += bytesTransferred;
            token.OffsetDelta += bytesTransferred;
            int usedByte = m_RquestProtocol.ProtocolProcess(buffer, token.OriginOffset, token.LastLength, out request);
            if (request != null && usedByte > 0)
            {
                while (request != null && usedByte > 0) //protocol parse success
                {
                    token.LastLength -= usedByte;
                    if (token.LastLength > 0)
                    {
                        Buffer.BlockCopy(buffer, token.OriginOffset + usedByte, buffer, token.OriginOffset, token.LastLength);
                        Array.Clear(buffer, token.OriginOffset + token.LastLength, usedByte);
                        token.OffsetDelta -= usedByte;
                    }
                    else
                    {
                        Array.Clear(buffer, token.OriginOffset, usedByte);
                        token.OffsetDelta = token.OriginOffset;
                        token.LastLength = 0;
                    }
                    //request message filter process
                    int index = 0;

                    while (index < m_RequestFilters.Count)
                    {
                        try
                        {
                            bool result = m_RequestFilters[index].ProcessFilter(ref request);
                            //to do use result
                        }
                        catch (Exception ex)
                        {
                            throw new FilterErrorException("request message filter process occur error", m_RequestFilters[index].FilterName, ex);
                        }
                        index++;
                    }


                    request.Context = new Dictionary<string, object>();
                    request.Context.Add("remoteEndPoint", remoteEndPoint);
                    request.RemoteIpEndPoint = new IPEndPoint(remoteEndPoint.Address, remoteEndPoint.Port); ;

                    //request message handler process
                    foreach (var handler in m_RequestHandlers)
                    {
                        if (handler.HandleCmdCode == request.CommandCode)
                        {
                            try
                            {
                                bool result = handler.ExecuteHandler(request, this);
                            }
                            catch (Exception ex)
                            {
                                m_Logger.ErrorLogger(ex.InnerException.Message, ex.InnerException.StackTrace);
                            }
                            break;
                        }
                    }
                    // continue process last bytes
                    usedByte = m_RquestProtocol.ProtocolProcess(buffer, token.OriginOffset, token.LastLength, out request);
                }
            }
            else if (request == null && usedByte > 0)
            {
                Array.Clear(buffer, token.OriginOffset, token.LastLength);
                token.Reset();
            }
            return token;
        }

        /// <summary>
        ///  send finished prcessm_RequestMsgFilter
        /// </summary>
        /// <param name="e"></param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {

            }
            else
            {
                CloseSession("send message to client fail", e);
            }
        }

        /// <summary>
        /// close session
        /// </summary>
        /// <param name="e"></param>
        public void CloseSession(string reason, SocketAsyncEventArgs socketEventArgs)
        {
            try
            {
                var token = socketEventArgs.UserToken as SocketAsyncToken;
                string TSessionID = token.TokenID;
                token.Reset();
                if (SessionClosed != null)
                {
                    IPEndPoint ipe = socketEventArgs.RemoteEndPoint as IPEndPoint;
                    string sessionID = string.Format("{0}:{1}", ipe.Address.ToString(), ipe.Port);
                    SessionClosed(this, new SessionCloseEventArgs(reason, TSessionID, socketEventArgs));
                }
            }
            catch { }
        }
        #endregion

        #region  ISocketSession Implement
        /// <summary>
        /// Get All Sesson
        /// </summary>
        /// <returns></returns>
        public override IList<string> GetAllSessionId()
        {
            return m_SocketServer.GetSessionIds();
        }

        /// <summary>
        /// send message to current session client
        /// </summary>
        /// <param name="message">message byte array</param>
        /// <returns></returns>
        public override bool SendSync(byte[] message, EndPoint remoteEndPoint)
        {
            if (m_ClientSocket != null)
            {
                return m_SocketAsyncSender.SendSync(message, remoteEndPoint);
            }
            return false;
        }

        /// <summary>
        ///  send message to specific session client
        /// </summary>
        /// <param name="sessionId">session Id</param>
        /// <param name="message">message byte array</param>
        /// <returns></returns>
        public override bool SendSync(string sessionId, byte[] message, EndPoint remoteEndPoint)
        {
            var session = m_SocketServer.GetSession(sessionId);
            if (session == null) return false;
            return session.SendSync(message, remoteEndPoint);
        }

        public override bool SendAsync(byte[] message, EndPoint remoteEndPoint)
        {
            if (m_ClientSocket != null)
            {
                return m_SocketAsyncSender.SendAsync(message, remoteEndPoint);
            }
            return false;
        }

        public override bool SendAsync(string sessionId, byte[] message, EndPoint remoteEndPoint)
        {
            var session = m_SocketServer.GetSession(sessionId);
            if (session == null) return false;
            return m_SocketAsyncSender.SendAsync(message, remoteEndPoint);
        }
        #endregion
    }
}
