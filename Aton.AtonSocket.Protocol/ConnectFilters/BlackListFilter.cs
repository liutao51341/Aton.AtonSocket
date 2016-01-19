using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Aton.AtonSocket.Core;

namespace Aton.AtonSocket.Protocol.ConnectFilters
{
    public class BlackListFilter : Core.Filter.IConnectFilter
    {
        public string FilterName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool ConnectFilter(IPEndPoint RemoteEndPoint)
        {
            throw new NotImplementedException();
        }
    }
}
