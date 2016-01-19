using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Core.EventArgs
{
    public class ServerStateChangedEventArgs: System.EventArgs
    {
        public ServerStatus SocketServerStatus { get; set; }
        public DateTime ChangeDateTime { get; set; }

        public ServerStateChangedEventArgs(ServerStatus status)
        {
            SocketServerStatus = status;
            ChangeDateTime = DateTime.Now;
        }
    }
}
