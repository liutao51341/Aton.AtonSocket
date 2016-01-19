using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aton.AtonSocket.Core.Filter;
using Aton.AtonSocket.Core.Config;
using Aton.AtonSocket.Core.EventArgs;
using Aton.AtonSocket.Core.Handler;

namespace Aton.AtonSocket.Core
{
    /// <summary>
    ///  socket server interface
    /// </summary>
    public interface ISocketServer
    {
        string ServerName { get; set; }

        string ServerDesc { get; set; }

        int MaxSessionCount { get; set; }

        ServerStatus SocketServerStauts { get; set; }

        //IRequestProtocol m_RquestProtocol { get; set; }

        //IList<IConnectFilter> m_ConnectFilters { get; set; }

        //IList<IRequestFilter> m_RequestFilters { get; set; }

        //IList<IRequestHandler> m_RequestHandlers { get; set; }

        ILogger m_Logger { get; set; }

        void initializeServer(ServerConfig config, IMsgProtocol protocol, IList<IConnectFilter> connectFilters, IList<IMsgFilter> requestFilters, IList<IMsgHandler> requestHandlers,ILogger logger);

        IList<string> GetSessionIds();

        ISocketSession GetSession(string sessionId);

        void RegisterSession(string sessionId, ISocketSession session);

        void RegisterRequestProtocol(IMsgProtocol protocol);

        void RegisterConnectFilter(IConnectFilter ConnectFilter);

        void RegisterRequestFilter(IMsgFilter msgFilter);

        void RegisterRequestHandler(IMsgHandler requestHandler);

        void Start();

        void Stop();

        event EventHandler<SessionCreatedEventArgs> SessionCreated;

        event EventHandler<ServerStateChangedEventArgs> ServerStateChanged;
    }
}
