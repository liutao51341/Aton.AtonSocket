using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aton.AtonSocket.Core;
using Aton.AtonSocket.Core.EventArgs;
using Aton.AtonSocket.Core.Utility;
using System.Net.Sockets;
using System.Net;
using Aton.AtonSocket.Core.Filter;
using Aton.AtonSocket.Core.Exceptions;
using Aton.AtonSocket.Core.Config;
using Aton.AtonSocket.Core.Handler;
using System.Collections.Concurrent;
using System.Threading;
using System.Timers;

namespace Aton.AtonSocket
{
    public class UdpSocketAsyncServer : SocketServerBase
    {
        int ServerDataSocketPort;

        Socket m_ListenSocket;

        Socket m_DataSockeet;

        SocketAsyncEventArgs m_ReadSocketAsyncEventArgs;

        SocketAsyncEventArgsPool m_ReadPool;

        BufferManager r_bufferManager;

        ConcurrentDictionary<string, UdpSocketAsyncBuffer> m_bufferManager;

        public override void initializeServer(ServerConfig config, IMsgProtocol protocol, IList<IConnectFilter> connectFilters, IList<IMsgFilter> requestFilters, IList<IMsgHandler> requestHandlers, ILogger logger)
        {
            base.initializeServer(config, protocol, connectFilters, requestFilters, requestHandlers, logger);

            m_ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //数据通讯socket
            m_DataSockeet = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //随机生成数据端口
            // ServerDataSocketPort = ServerPort + (new Random()).Next(1, 100);
            ServerDataSocketPort = ServerPort + 1;

            r_bufferManager = new BufferManager(BufferSize * MaxSessionCount, BufferSize);
            m_ReadPool = new SocketAsyncEventArgsPool(MaxSessionCount);
            m_ReadPool.Initize(r_bufferManager);

            m_bufferManager = new ConcurrentDictionary<string, UdpSocketAsyncBuffer>();
        }

        public override void Start()
        {
            try
            {
                //bind listen socket
                m_ListenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
                m_ListenSocket.EnableBroadcast = true;
                m_ListenSocket.Blocking = false;
                m_ListenSocket.Bind(new IPEndPoint(IPAddress.Parse(ServerIpAddress), ServerPort));

                //bind data socket
                m_DataSockeet.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                m_DataSockeet.Blocking = false;
                m_DataSockeet.Bind(new IPEndPoint(IPAddress.Parse(ServerIpAddress), ServerDataSocketPort));

                m_ReadSocketAsyncEventArgs = m_ReadPool.Pop();
                m_ReadSocketAsyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(receiveSocketArgs_Completed);
            }
            catch (Exception ex)
            {
                m_Logger.ErrorLogger("Start Server Fail",ex.StackTrace);
                return;
            }

            StartReceive(m_ReadSocketAsyncEventArgs);
        }

        public void StartReceive(SocketAsyncEventArgs e)
        {
            if (e.RemoteEndPoint == null)
                e.RemoteEndPoint = new IPEndPoint(IPAddress.Any, ServerPort);

            if (!m_ListenSocket.ReceiveFromAsync(e))
            {
                processConnect(e, false);
            }
        }

        void receiveSocketArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.ReceiveFrom:
                    if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                    {
                        processConnect(e, false);
                    }
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive");
            }
        }

        /// <summary>
        /// 接收完成时处理请求。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void processConnect(SocketAsyncEventArgs e, bool isExtRecvConect)
        {
            IPEndPoint ipe = e.RemoteEndPoint as IPEndPoint;
            string sessionID = string.Format("{0}:{1}", ipe.Address.ToString(), ipe.Port);

            foreach (IConnectFilter filter in m_ConnectFilters)
            {
                if (!filter.ConnectFilter(e.RemoteEndPoint as IPEndPoint))
                {
                    m_Logger.WarnningLogger(string.Format("Filter by {0},refuse client {1}:{2} connecting,force disconnected!", filter.FilterName, ipe.Address.ToString(), ipe.Port));
                    return;
                }
            }

            UdpSocketAsyncSession session;
            if (dictSession.ContainsKey(sessionID))//已经连接的客户端，
            {
                string response = string.Format("{0}:{1}", ServerIpAddress, ServerDataSocketPort);
                m_ListenSocket.SendTo(ASCIIEncoding.Default.GetBytes(response), e.RemoteEndPoint as IPEndPoint);
                //接收下次连接
                if (e != null) StartReceive(e);
            }
            else//新客户端 初始化
            {
                session = new UdpSocketAsyncSession(m_DataSockeet, this, m_RquestProtocol, m_RequestFilters, m_RequestHandlers, m_Logger);
                session.SessionClosed += Session_SessionClosed;
                session.SessionId = sessionID;

                m_Logger.InfoLogger(string.Format("[{0}] receive UDP Connect from:{1}:{2}",
                     DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                    (e.RemoteEndPoint as IPEndPoint).Address.ToString(),
                    (e.RemoteEndPoint as IPEndPoint).Port));

                FireSessionCreatedEvent(session);
                RegisterSession(session.SessionId, session);

                var token = e.UserToken as SocketAsyncToken;
                token.TokenID = sessionID;

                //注销当前连接的事件绑定，将对象放入新线程绑定处理
                e.Completed -= receiveSocketArgs_Completed;
                UdpSocketAsyncBuffer buf = new UdpSocketAsyncBuffer();
                buf.buffer = new byte[BufferSize];
                m_bufferManager.AddOrUpdate(sessionID, buf, (key, oldValue) => buf);

                AsyncUtility.Run(() => session.StartSession(e, m_bufferManager));

                //返回数据地址
                string response = string.Format("{0}:{1}", ServerIpAddress, ServerDataSocketPort);
                m_ListenSocket.SendTo(ASCIIEncoding.Default.GetBytes(response), e.RemoteEndPoint as IPEndPoint);

                //开启新的继续接收连接
                var args = m_ReadPool.Pop();
                args.Completed += receiveSocketArgs_Completed;
                if (args != null) StartReceive(args);
            }
        }


        private void Session_SessionClosed(object sender, SessionCloseEventArgs e)
        {
            m_ReadPool.Push(e.SocketEventArgsRecv);
             
            lock (dictSession)
            {
                ISocketSession ss;
                dictSession.TryRemove(e.SessionID, out ss);
            }

            lock (m_bufferManager)
            {
                UdpSocketAsyncBuffer ub;
                m_bufferManager.TryRemove(e.SessionID, out ub);
            }

            m_Logger.InfoLogger(string.Format("Session:{0} Timeout,GC SocketAsyncEventArgs", e.SessionID));
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public override void Stop()
        {
            m_ListenSocket.Close();
            SocketServerStauts = ServerStatus.Stopped;
        }

        /// <summary>
        /// 获取所有客户端会话
        /// </summary>
        /// <returns></returns>
        public override IList<string> GetSessionIds()
        {
            return dictSession.Keys.ToList();
        }

        /// <summary>
        /// 获取会话
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public override ISocketSession GetSession(string sessionId)
        {
            ISocketSession session = null;
            if (dictSession.TryGetValue(sessionId, out session))
            {
                return session;
            }
            return session;
        }

    }
}
