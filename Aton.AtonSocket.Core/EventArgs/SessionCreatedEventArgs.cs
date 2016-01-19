using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Core.EventArgs
{

    /// <summary>
    /// 会话创建事件
    /// </summary>
    public class SessionCreatedEventArgs : System.EventArgs
    {
        public ISocketSession Session { get; set; }

        public SessionCreatedEventArgs(ISocketSession session)
        {
            Session = session;
        }
    }

}