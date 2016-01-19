using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Aton.AtonSocket.Core.Config;

using System.Net;
using System.Threading;
using Aton.AtonSocket.Core.Filter;
using Aton.AtonSocket.Core;
using Aton.AtonSocket.Core.Utility;
using Aton.AtonSocket.Core.EventArgs;
using Aton.AtonSocket.Core.Handler;
using Aton.AtonSocket.Core.Exceptions;


namespace Aton.AtonSocket
{
    public class TcpAsyncSocketServer : SocketServerBase
    {

        SocketAsyncEventArgsPool m_ReadWritePool;

        BufferManager bufferManager;

        Socket m_ListenerSocket;
 
        public override void initializeServer( ServerConfig config,  IMsgProtocol protocol,  IList<IConnectFilter> connectFilters,  IList<IMsgFilter> requestFilters,  IList<IMsgHandler> requestHandlers, ILogger logger)
        {
            base.initializeServer(config, protocol, connectFilters, requestFilters, requestHandlers, logger);
 
            bufferManager = new BufferManager(BufferSize * MaxSessionCount, BufferSize);
            m_ReadWritePool = new SocketAsyncEventArgsPool(MaxSessionCount);
            m_ReadWritePool.Initize(bufferManager);
          
        }

        public override void Start()
        {
            if (SocketServerStauts == ServerStatus.NotInit)
            {
                m_Logger.ErrorLogger("tcp socket server can not start because the server is uninitilized","");
                throw new SocketServerErrorException("tcp socket server can not start because the server is uninitilized",ServerName);
            }

            m_ListenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_ListenerSocket.Bind(new IPEndPoint(IPAddress.Parse(ServerIpAddress), ServerPort));

            m_ListenerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            m_ListenerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

            m_ListenerSocket.Listen(BackLogCount);

            StartAccpt(null);
            m_Logger.InfoLogger(string.Format("Start {0} Server Success...",ServerName));
        }


        private void StartAccpt(SocketAsyncEventArgs listenAsyncEventArgs)
        {
            if (listenAsyncEventArgs == null)
            {
                listenAsyncEventArgs = new SocketAsyncEventArgs();
                listenAsyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(listenAsyncEventArgs_Completed);
            }
            else
            {
                listenAsyncEventArgs.AcceptSocket = null; 
            }

            bool isPeeding = m_ListenerSocket.AcceptAsync(listenAsyncEventArgs);
            if (!isPeeding)
            {
                ProcessAccept(listenAsyncEventArgs);
            }
        }

        void listenAsyncEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            TcpSocketAsyncSession session = new TcpSocketAsyncSession(e.AcceptSocket, this,m_RquestProtocol,m_RequestFilters,m_RequestHandlers,m_Logger);
            session.RemoteEndPoint = e.AcceptSocket.RemoteEndPoint as IPEndPoint;
            session.SessionClosed += new EventHandler<SessionCloseEventArgs>(session_SessionClosed);
            session.SessionId = string.Format("{0}:{1}",session.RemoteEndPoint.Address.ToString(),session.RemoteEndPoint.Port);
            m_Logger.InfoLogger(string.Format("[{0}] Accept New Connect From:{1}",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),session.RemoteEndPoint.Address.ToString()));

            foreach (IConnectFilter filter in m_ConnectFilters)
            {
                if (!filter.ConnectFilter(e.RemoteEndPoint as IPEndPoint))
                {
                    var ip = e.RemoteEndPoint as IPEndPoint;
                    m_Logger.WarnningLogger(string.Format("Filter by {0},refuse client {1}:{2} connecting,force disconnected!", filter.FilterName, ip.Address.ToString(), ip.Port));
                    try
                    {
                        e.AcceptSocket.Shutdown(SocketShutdown.Both);
                        e.AcceptSocket.Close();
                    }
                    catch (Exception) { }
                    return;
                }
            }

            FireSessionCreatedEvent(session);

            RegisterSession(session.SessionId, session);

            var recv = m_ReadWritePool.Pop();
            if (recv != null)
            {
                AsyncUtility.Run(() => session.StartSession(recv));
            }

            StartAccpt(e);
        }

        private void session_SessionClosed(object sender, SessionCloseEventArgs e)
        {
            TcpSocketAsyncSession session = sender as TcpSocketAsyncSession;
            m_ReadWritePool.Push(e.SocketEventArgsRecv);
            ISocketSession ise;
            if (dictSession.TryRemove(session.SessionId, out ise))
            {  
                m_Logger.InfoLogger("Remote Client: " + session.RemoteEndPoint.Address.ToString()+":"+session.RemoteEndPoint.Port + " closed, " + e.Reason);
            }
        }

        public override void Stop()
        {
            if (m_ListenerSocket != null)
                m_ListenerSocket.Close();
            m_ListenerSocket = null;
            SocketServerStauts = ServerStatus.Stopped;
        }

        public override IList<string> GetSessionIds()
        {
            return dictSession.Keys.ToList();
        }

        public override ISocketSession GetSession(string sessionId)
        {
            ISocketSession session=null;
            if (dictSession.TryGetValue(sessionId, out session))
            {
                return session;
            }
            return session;
        }
    }
}
