using Aton.AtonSocket.Core;
using Aton.AtonSocket.Core.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Protocol.RequestHandlers
{
    public class BroadcastHandler : IMsgHandler
    {
        public string HandlerName
        {
            get
            {
                return "Broadcast";
            }
        }

        public string HandleCmdCode
        {
            get { return ""; }
        }

        public bool ExecuteHandler(IMsg requestInfo, ISocketSession session)
        {
            foreach (var s in session.GetAllSessionId())
            {
                session.SendAsync(s, requestInfo.Data, requestInfo.RemoteIpEndPoint);
            }

            return true;
        }
    }
}