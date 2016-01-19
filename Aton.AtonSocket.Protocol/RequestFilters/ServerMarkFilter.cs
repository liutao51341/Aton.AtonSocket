using Aton.AtonSocket.Core;
using Aton.AtonSocket.Core.Filter;
using Aton.AtonSocket.Core.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Protocol.Handlers
{
    public class ServerMarkFilter : IMsgFilter
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
            if (requestMsg != null)
            {
                byte[] mark = ASCIIEncoding.ASCII.GetBytes("-From Aton Srv.");
                byte[] dest = new byte[requestMsg.Data.Length + mark.Length];
                Array.Copy(requestMsg.Data, 0, dest, 0, requestMsg.Data.Length);
                Array.Copy(mark, 0, dest, requestMsg.Data.Length, mark.Length);
                requestMsg.Data = dest;
            }
            return true;
        }
    }
}