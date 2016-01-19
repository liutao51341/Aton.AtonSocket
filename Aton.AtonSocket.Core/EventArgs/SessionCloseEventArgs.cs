using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Aton.AtonSocket.Core.EventArgs
{
    /// <summary>
    /// 会话关闭事件参数
    /// </summary>
    public class SessionCloseEventArgs : System.EventArgs
    {
        public string Reason { get; set; }

        public string SessionID { get; set; }

        public SocketAsyncEventArgs SocketEventArgsRecv { get; set; }

        public SessionCloseEventArgs(string reason,string sessionID, SocketAsyncEventArgs socketEventArgsRecv)
        {
            Reason = reason;
            SocketEventArgsRecv = socketEventArgsRecv;
            SessionID = sessionID;
        }
    }
}
