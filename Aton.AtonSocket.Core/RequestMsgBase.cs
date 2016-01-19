using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Core
{
    /// <summary>
    /// 基础消息
    /// </summary>
   public  class RequestMsgBase:IRequestMsg
    {
        public override void Reset() {Context.Clear();Data = null;}
    }
}
