using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Core.Config
{
    /// <summary>
    /// socket服务配置
    /// </summary>
    public class ServerConfig
    {
        /// <summary>
        /// 服务模式
        /// </summary>
        public ServerMode ServerMode { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServerName { get; set; }
        /// <summary>
        /// 服务描述
        /// </summary>
        public string ServerDesc { get; set; }
        /// <summary>
        /// 服务端IP地址
        /// </summary>
        public string ServerIpAddress { get; set; }
        /// <summary>
        /// 服务端口
        /// </summary>
        public int ServerPort { get; set; }
        /// <summary>
        /// 缓冲区大小
        /// </summary>
        public int BufferSize { get; set; }
        /// <summary>
        /// 最大监听数
        /// </summary>
        public int BackLogCount { get; set; }
        /// <summary>
        /// 最大会话处理数
        /// </summary>
        public int MaxSessionCount { get; set; }
        /// <summary>
        ///  会话过期时间
        /// </summary>
        public int SessionTimeout { get; set; }
    }
}
