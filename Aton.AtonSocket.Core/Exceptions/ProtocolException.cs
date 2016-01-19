using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Core.Exceptions
{
    public class ProtocolException:Exception
    {
         public string ProtocolName { get; set; }

        public ProtocolException(string message, string protocolName)
            : base(message)
        {
            ProtocolName = protocolName;
        }

        public ProtocolException(string message, string protocolName, Exception innerException)
            : base(message, innerException)
        {
            ProtocolName = protocolName;
        }
    }
}
