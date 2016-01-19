using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Core
{
    /// <summary>
    /// 协议处理器基类
    /// </summary>
    public abstract class ProtocolBase:IMsgProtocol
    {
        protected Dictionary<string, Type> m_RequestMsgs { get; set; }

        public void Init(Dictionary<string, Type> requestMsgs) { m_RequestMsgs = requestMsgs; }

        public abstract int ProtocolProcess(byte[] buffer, int offset, int length, out IMsg requestInfo);

        protected IMsg CreateRequestInfoInstance(string commandCode)
        {
            if (m_RequestMsgs.ContainsKey(commandCode))
            {
                return Activator.CreateInstance( m_RequestMsgs[commandCode]) as IMsg;
            }
            return new MsgBase();
        }
    }
}
