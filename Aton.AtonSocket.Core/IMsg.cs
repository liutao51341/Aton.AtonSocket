using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Aton.AtonSocket.Core
{
    /// <summary>
    /// 消息接口
    /// </summary>
    public abstract class IMsg
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public IMsg(string cmdCode="") { CommandCode = cmdCode; }
        /// <summary>
        /// 命令号
        /// </summary>
        public  string CommandCode { get; set; }
        /// <summary>
        /// 消息数据 
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// 附带参数上下文
        /// </summary>
        public Dictionary<string, object> Context { get; set; }
        /// <summary>
        /// 重置
        /// </summary>
        public abstract void Reset();

        public IPEndPoint RemoteIpEndPoint { get; set; }
        /// <summary>
        /// 序列化为字节流
        /// </summary>
        /// <returns></returns>
        public virtual byte[] SerializeToBytes() { return Data; }
    }
}
