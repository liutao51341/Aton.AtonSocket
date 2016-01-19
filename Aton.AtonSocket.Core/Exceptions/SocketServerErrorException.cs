using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Core.Exceptions
{
    public  class SocketServerErrorException : Exception
    {
        public string ServerName { get; set; }

        public SocketServerErrorException(string message, string serverName)
            : base(message)
        {
            ServerName = serverName;
        }

        public SocketServerErrorException(string message, string serverName, Exception innerException)
            : base(message, innerException)
        {
            ServerName = serverName;
        }
    }
}
