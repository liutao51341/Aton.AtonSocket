using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Aton.AtonSocket.Core.EventArgs
{
    public class SessionConnectedEventArgs : System.EventArgs
    {
        public IPEndPoint RemoteEndPoint { get; set; }
        public DateTime ConnectDateTime { get; set; }

        public SessionConnectedEventArgs(IPEndPoint remoteEndPoint)
        {
            RemoteEndPoint = remoteEndPoint;
            ConnectDateTime = DateTime.Now;
        }
    }
}
