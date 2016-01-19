using Aton.AtonSocket.Core;
using Aton.AtonSocket.Core.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Protocol.RequestFilters
{
    public class DataDecryptFilter : IMsgFilter
    {
        public string FilterName
        {
            get
            {
                return "ServerMark";
            }
        }

        public int IndexNo
        {
            get
            {
                return 99;
            }
        }

        public bool ProcessFilter(ref IMsg requestMsg)
        {


            byte[] dest = ASCIIEncoding.ASCII.GetBytes(Convert.ToBase64String(requestMsg.Data));

            requestMsg.Data = dest;

            return true;
        }
    }
}