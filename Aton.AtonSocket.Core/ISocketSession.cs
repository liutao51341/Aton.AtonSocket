using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aton.AtonSocket.Core.Filter;
using System.Net;
using Aton.AtonSocket.Core.Handler;

namespace Aton.AtonSocket.Core
{
    /// <summary>
    /// 异步会话接口
    /// </summary>
    public interface ISocketSession
    {
        IPEndPoint RemoteEndPoint { get; }

        IMsgProtocol m_RquestProtocol { get; set; }
 
        IList<IMsgFilter> m_RequestFilters { get; set; }

        IList<IMsgHandler> m_RequestHandlers { get; set; }

        ILogger m_Logger { get; set; }
 
        string SessionId { get; set; }
 
        IList<string> GetAllSessionId();
 
        bool SendSync(byte[] message, EndPoint remoteEndPoint);
 
        bool SendSync(string sessionId, byte[] message, EndPoint remoteEndPoint);
 
        bool SendAsync(byte[] message, EndPoint remoteEndPoint);
 
        bool SendAsync(string sessionId, byte[] message, EndPoint remoteEndPoint);
    }
}
