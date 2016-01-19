using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Core.Exceptions
{
    public class FilterErrorException : Exception
    {
        public string FilterName { get; set; }

        public FilterErrorException(string message, string protocolName)
            : base(message)
        {
            FilterName = protocolName;
        }

        public FilterErrorException(string message, string filterName, Exception innerException)
            : base(message, innerException)
        {
            FilterName = filterName;
        }
    }
}