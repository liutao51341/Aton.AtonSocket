using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Aton.AtonSocket.Core.Handler;
using Aton.AtonSocket.Core;
using Aton.AtonSocket.Core.Filter;

namespace Aton.AtonSocket
{
    /// <summary>
    ///  socket session base class
    /// </summary>
    abstract class SocketSessionBase : ISocketSession
    {
        public string SessionId { get; set; }

        public IPEndPoint RemoteEndPoint { get; internal set; }
        public IMsgProtocol m_RquestProtocol { get; set; }

        public IList<IMsgFilter> m_RequestFilters { get; set; }

        public IList<IMsgHandler> m_RequestHandlers { get; set; }

        public ILogger m_Logger { get; set; }

        public SocketSessionBase(IMsgProtocol protocol,  IList<IMsgFilter> requestFilters, IList<IMsgHandler> requestHandlers, ILogger logger)
        {
            SessionId = null;
            m_RquestProtocol = protocol;
            m_RequestHandlers = requestHandlers;
            m_RequestFilters = requestFilters;
            m_Logger = logger;
        }

        public abstract IList<string> GetAllSessionId();

        public abstract bool SendSync(byte[] message,EndPoint remoteEndPoint);

        public abstract bool SendSync(string sessionId, byte[] message, EndPoint remoteEndPoint);

        public abstract bool SendAsync(byte[] message, EndPoint remoteEndPoint);

        public abstract bool SendAsync(string sessionId, byte[] message, EndPoint remoteEndPoint);
 
    }
}
