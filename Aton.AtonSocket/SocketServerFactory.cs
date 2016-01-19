using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aton.AtonSocket.Core;

namespace Aton.AtonSocket
{
    internal class SocketServerFactory
    {
        public static SocketServerBase CreateSocketServer(ServerMode serverMode)
        {
            if (serverMode == ServerMode.TCP)
                return new TcpAsyncSocketServer();
            else if (serverMode == ServerMode.UDP)
                return new UdpSocketAsyncServer();
            else
                return null;
        }
    }
}
