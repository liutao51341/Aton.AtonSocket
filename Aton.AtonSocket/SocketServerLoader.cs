using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aton.AtonSocket.Core.Filter;
using Aton.AtonSocket.Core;
using Aton.AtonSocket.Core.Config;
using Aton.AtonSocket.Core.Handler;
using Aton.AtonSocket.Core.EventArgs;
using Aton.AtonSocket.Core.Loggers;
using Aton.AtonSocket.Protocol;
using Aton.AtonSocket.Protocol.RequestHandlers;

namespace Aton.AtonSocket
{
    /// <summary>
    /// socket server bootstrap use to manager socket server
    /// </summary>
    public class SocketServerLoader
    {
        #region SocketServerGuard
        /// <summary>
        /// server configuration
        /// </summary>
        ServerConfig m_ServerConfig;
        /// <summary>
        /// connect filter list 
        /// </summary>
        List<IConnectFilter> m_ConnectFilterList;
        /// <summary>
        /// protocol parser object
        /// </summary>
        IMsgProtocol m_Protocol;
        /// <summary>
        /// request handler list
        /// </summary>
        List<IMsgHandler> m_RequestHandlers;
        /// <summary>
        /// request handler list
        /// </summary>
        List<IMsgFilter> m_RequestFilters;
        /// <summary>
        /// socket server object
        /// </summary>
        ISocketServer m_SocketServer;
        #endregion

        /// <summary>
        /// 初始化加载器
        /// </summary>
        public void initializeSocketServer()
        {
            //读取配置

            m_Protocol = new  FixSizeProtocol(5);
            m_RequestHandlers = new List<IMsgHandler>();   
            m_RequestHandlers.Add(new ResponseHandler());

            m_RequestFilters = new List<IMsgFilter>();
           // m_RequestFilters.Add(new Protocol.RequestFilters.DateEcrptyFilter());
           // m_RequestFilters.Add(new Protocol.Handlers.ServerMarkFilter());
#if DEBUG
            m_ServerConfig = new ServerConfig();
            m_ServerConfig.BackLogCount = 5;
            m_ServerConfig.BufferSize = 1024;
            m_ServerConfig.MaxSessionCount = 10;
            m_ServerConfig.ServerIpAddress = "192.168.10.2";
            m_ServerConfig.ServerMode = ServerMode.UDP;
            m_ServerConfig.ServerPort = 6000;
            m_ServerConfig.SessionTimeout = 20*60;
            m_ServerConfig.ServerName = "ATONServer";
            m_ConnectFilterList = new List<IConnectFilter>();
#endif
            m_SocketServer = SocketServerFactory.CreateSocketServer(m_ServerConfig.ServerMode);
            m_SocketServer.initializeServer(m_ServerConfig, m_Protocol, m_ConnectFilterList, m_RequestFilters, m_RequestHandlers, new ConsoleLogger(LoggerLevelE.INFO));
        }

        public bool SetProtocol(IMsgProtocol protocol)
        {
            m_Protocol = protocol;
            return protocol != null;
        }

        public bool AddConnectFilter(IConnectFilter connectFilter)
        {
            m_ConnectFilterList.Add(connectFilter);
            return connectFilter != null;
        }

        public bool AddRequestHandler(IMsgHandler requestHandler)
        {
            m_RequestHandlers.Add(requestHandler);
            return requestHandler != null;
        }

        public bool AddRequestFilter(IMsgFilter requestFilter)
        {
            m_RequestFilters.Add(requestFilter);
            return requestFilter != null;
        }

        public void Start()
        {
            m_SocketServer.Start();
        }

        public void Stop()
        {
            m_SocketServer.Stop();
        }
    }
}
