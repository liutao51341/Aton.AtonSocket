using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket
{
    internal class UdpSocketAsyncBuffer
    {
        public byte[] buffer { get; set; }

        public SocketAsyncToken SocketToken {get;set;}

        public DateTime LastSocketTime { get; set; }

        public UdpSocketAsyncBuffer() { SocketToken = new SocketAsyncToken();LastSocketTime = DateTime.Now;  }
    }
}
