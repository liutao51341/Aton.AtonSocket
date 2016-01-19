using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aton.AtonSocket.Core.Handler;
using Aton.AtonSocket.Core;

namespace Aton.AtonSocket.Protocol.RequestHandlers
{
    public class ResponseHandler : IMsgHandler
    {
        public string HandlerName
        {
            get
            {
                return "Rsp";
            }
        }

        public string HandleCmdCode
        {
            get { return ""; }
        }

        public bool ExecuteHandler(IMsg requestInfo, ISocketSession session)
        {
            byte[] b= new byte[2048];
            for(int i=0;i< 2048; i++)
            {
                b[i] = 127; 
            }
            session.SendAsync(requestInfo.Data, requestInfo.RemoteIpEndPoint);
            return true;
        }
    }
}
