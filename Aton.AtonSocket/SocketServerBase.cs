using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aton.AtonSocket.Core.Filter;
using Aton.AtonSocket.Core.Config;
using Aton.AtonSocket.Core;
using System.Collections.Concurrent;
using Aton.AtonSocket.Core.EventArgs;
using Aton.AtonSocket.Core.Handler;

namespace Aton.AtonSocket
{
    /// <summary>
    /// socket server base class
    /// </summary>
    public abstract class SocketServerBase : ISocketServer
    {
        /// <summary>
        /// session created event
        /// </summary>
        public event EventHandler<SessionCreatedEventArgs> SessionCreated;
        /// <summary>
        /// server state changed event
        /// </summary>
        public event EventHandler<ServerStateChangedEventArgs> ServerStateChanged;
        /// <summary>
        /// server name
        /// </summary>
        public string ServerName { get; set; }
        /// <summary>
        /// server description
        /// </summary>
        public string ServerDesc { get; set; }
        /// <summary>
        /// server ip address
        /// </summary>
        public string ServerIpAddress { get; set; }
        /// <summary>
        /// server port
        /// </summary>
        public int ServerPort { get; set; }
        /// <summary>
        /// buffer size
        /// </summary>
        public int BufferSize { get; set; }
        /// <summary>
        /// session timeout
        /// </summary>
        public int SessionTimeout { get; set; }
        /// <summary>
        /// max listen count
        /// </summary>
        public int BackLogCount { get; set; }
        /// <summary>
        /// support max session count
        /// </summary>
        public int MaxSessionCount { get; set; }
        /// <summary>
        /// server status
        /// </summary>
        public ServerStatus SocketServerStauts { get; set; }
        /// <summary>
        /// connect filter list
        /// </summary>
        protected IList<IConnectFilter> m_ConnectFilters{get;  set;}
        /// <summary>
        /// request filter list
        /// </summary>
        protected IList<IMsgFilter> m_RequestFilters { get; set; }
        /// <summary>
        /// request handler list
        /// </summary>
        protected IList<IMsgHandler> m_RequestHandlers { get; set; }
        /// <summary>
        /// protocol process
        /// </summary>
        protected IMsgProtocol m_RquestProtocol { get; set; }
        /// <summary>
        /// logger
        /// </summary>
        public ILogger m_Logger { get; set; }

        /// <summary>
        /// session list
        /// </summary>
        protected ConcurrentDictionary<string, ISocketSession> dictSession { get; set; }

  

        public SocketServerBase()
        {
            SocketServerStauts = ServerStatus.NotInit;
            dictSession = new ConcurrentDictionary<string, ISocketSession>();
        }

        public virtual void initializeServer(ServerConfig config, IMsgProtocol protocol, IList<IConnectFilter> connectFilters, IList<IMsgFilter> requestFilters, IList<IMsgHandler> requestHandlers,ILogger logger)
        {
            ServerName = config.ServerName;
            ServerDesc = config.ServerDesc;
            ServerIpAddress = config.ServerIpAddress;
            ServerPort = config.ServerPort;
            BufferSize = config.BufferSize;
            BackLogCount = config.BackLogCount;
            MaxSessionCount = config.MaxSessionCount;
            SessionTimeout = config.SessionTimeout;
            //init connect filter
            if (connectFilters != null)
                m_ConnectFilters = connectFilters;
            else
                m_ConnectFilters = new List<IConnectFilter>();
            //init request filter
            if (requestFilters != null)
                m_RequestFilters = requestFilters.OrderBy(n => n.IndexNo).ToList();
            else
                m_RequestFilters = new List<IMsgFilter>();
            //init handler and protocol
            m_Logger = logger;
            if (requestHandlers != null && protocol != null && logger!=null)
            {
                m_RequestHandlers = requestHandlers;
                m_RquestProtocol = protocol;
                SocketServerStauts = ServerStatus.initialized;
                FireServerStateChangedEvent(SocketServerStauts);
            }
        }

        public abstract IList<string> GetSessionIds();

        public abstract ISocketSession GetSession(string sessionId);

        public abstract void Start();

        public abstract void Stop();

        public virtual void RegisterSession(string sessionId, ISocketSession session)
        {
            dictSession.TryAdd(sessionId, session);
        }

        public virtual void RegisterConnectFilter(IConnectFilter connectFilter)
        {
            m_ConnectFilters.Add(connectFilter);
        }

        public virtual void RegisterRequestFilter(IMsgFilter msgFilter)
        {
            m_RequestFilters.Add(msgFilter);
            m_RequestFilters = m_RequestFilters.OrderBy(n => n.IndexNo).ToList();
        }

        public virtual void RegisterRequestHandler(IMsgHandler requestHandler)
        {
            m_RequestHandlers.Add(requestHandler);
        }

        public void RegisterRequestProtocol(IMsgProtocol protocol)
        {
            m_RquestProtocol = protocol;
        }

        public virtual void FireSessionCreatedEvent(ISocketSession session)
        {
            if (SessionCreated != null) SessionCreated(this, new SessionCreatedEventArgs(session));
        }

        public virtual void FireServerStateChangedEvent(ServerStatus status)
        {
            if (ServerStateChanged != null) ServerStateChanged(this, new ServerStateChangedEventArgs(status));
        }
 
    }
}
