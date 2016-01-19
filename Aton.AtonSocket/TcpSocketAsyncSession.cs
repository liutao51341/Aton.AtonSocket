using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

using Aton.AtonSocket.Core;
using Aton.AtonSocket.Core.EventArgs;
using Aton.AtonSocket.Core.Exceptions;
using Aton.AtonSocket.Core.Filter;
using Aton.AtonSocket.Core.Handler;

namespace Aton.AtonSocket
{
    /// <summary>
    /// Socket Async Session
    /// </summary>
    internal class TcpSocketAsyncSession : SocketSessionBase
    {
        #region Session Param
        /// <summary>
        /// Socket Server object
        /// </summary>
        ISocketServer m_SocketServer { get; set; }
        /// <summary>
        ///  Client Socket object
        /// </summary>
        Socket m_ClientSocket { get; set; }
        /// <summary>
        /// SAEA object for async send
        /// </summary>
        SocketAsyncEventArgs m_SocketEventArgSend { get; set; }

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
        internal TcpSocketAsyncSession(Socket socket, ISocketServer socketServer, IMsgProtocol protocol, IList<IMsgFilter> requestFilters, IList<IMsgHandler> requestHandlers, ILogger logger)
            : base(protocol, requestFilters, requestHandlers, logger)
        {
            m_ClientSocket = socket;
            m_SocketServer = socketServer;
            m_Logger = logger;
        }

        /// <summary>
        /// start session running in new task
        /// </summary>
        /// <param name="socketEventArgRecv">SAEA for receive </param>
        internal void StartSession(SocketAsyncEventArgs socketEventArgRecv)
        {
            //first response
            socketEventArgRecv.Completed += new EventHandler<SocketAsyncEventArgs>(SocketEventArgs_Completed);
            StartReceive(socketEventArgRecv);

        }

        /// <summary>
        /// start async receive message from client by post receive operation
        /// </summary>
        /// <param name="socketEventArgRecv">saea for receive</param>
        internal void StartReceive(SocketAsyncEventArgs socketEventArgRecv)
        {
            bool isPeeding = m_ClientSocket.ReceiveAsync(socketEventArgRecv);
            if (!isPeeding)
            {
                ProcessReceive(socketEventArgRecv);
            }
        }

        /// <summary>
        /// receive message async complete process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SocketEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Receive)
                ProcessReceive(e);
            else
                CloseSession("receive complete is  invalid operation", e);
        }

        /// <summary>
        /// process received message
        /// </summary>
        /// <param name="e">saea object</param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success && m_ClientSocket != null)
            {
                try
                {
                    SocketAsyncToken token = e.UserToken as SocketAsyncToken;
                    e.UserToken = ReceiveMessageHandler(e.Buffer, e.BytesTransferred, token);
                    e.SetBuffer(token.OffsetDelta, token.BufferSize - token.LastLength);
                }
                catch (ProtocolException ex)
                {
                    m_Logger.ErrorLogger(ex.Message, ex.StackTrace);
                }
                catch (MsgHandlerException ex)
                {
                    m_Logger.ErrorLogger(ex.Message, ex.StackTrace);
                }
                catch (Exception ex)
                {
                    m_Logger.ErrorLogger(ex.Message, ex.StackTrace);
                }
                StartReceive(e);
            }
            else
            {
                CloseSession("receive message from cient fail,force disconnect", e);
            }
        }

        /// <summary>
        ///  use protocol and handler to process received message 
        /// </summary>
        /// <param name="buffer">message buffer</param>
        /// <param name="bytesTransferred">received message length</param>
        /// <param name="token">user token</param>
        /// <returns>user token for next receive </returns>
        private SocketAsyncToken ReceiveMessageHandler(byte[] buffer, int bytesTransferred, SocketAsyncToken token)
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

                    //request message handler process
                    request.Context = new Dictionary<string, object>();
                    request.Context.Add("remoteEndPoint", RemoteEndPoint);
                    request.RemoteIpEndPoint = RemoteEndPoint;

                    foreach (var handler in m_RequestHandlers)
                    {
                        if (handler.HandleCmdCode == request.CommandCode)
                        {
                            try
                            {
                                bool result = handler.ExecuteHandler(request, this);
                                //to do use result
                            }
                            catch (Exception ex)
                            {
                                throw new MsgHandlerException("request handler  process occur error", handler.HandlerName, ex);
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
        private void CloseSession(string reason, SocketAsyncEventArgs socketEventArgs)
        {
            try
            {
                m_ClientSocket.Shutdown(SocketShutdown.Both);
                m_ClientSocket.Close();
                m_ClientSocket = null;
                (socketEventArgs.UserToken as SocketAsyncToken).Reset();
            }
            catch { }

            if (SessionClosed != null)
            {
                SessionClosed(this, new SessionCloseEventArgs(reason,SessionId, socketEventArgs));
            }
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
        public override bool SendSync(byte[] message,EndPoint remoteEndPoint=null)
        {
            if (m_ClientSocket != null && m_ClientSocket.Connected)
            {
                return m_ClientSocket.Send(message) > 0;
            }
            return false;
        }

        /// <summary>
        ///  send message to specific session client
        /// </summary>
        /// <param name="sessionId">session Id</param>
        /// <param name="message">message byte array</param>
        /// <returns></returns>
        public override bool SendSync(string sessionId, byte[] message, EndPoint remoteEndPoint = null)
        {
            var session = m_SocketServer.GetSession(sessionId);
            if (session == null) return false;
            return session.SendSync(message,RemoteEndPoint);
        }

        public override bool SendAsync(byte[] message, EndPoint remoteEndPoint = null)
        {
            return SendSync(message);
        }

        public override bool SendAsync(string sessionId, byte[] message, EndPoint remoteEndPoint = null)
        {
            return SendSync(sessionId,message); ;
        }
        #endregion
    }
}
