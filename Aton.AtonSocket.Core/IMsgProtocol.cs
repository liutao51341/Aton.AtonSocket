using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Core
{
    /// <summary>
    /// 协议解析器接口
    /// </summary>
    public interface IMsgProtocol
    {
        /// <summary>
        /// 协议解析处理
        /// </summary>
        /// <param name="buffer">缓存</param>
        /// <param name="offset">偏移量</param>
        /// <param name="length">长度</param>
        /// <param name="request">解析后对象</param>
        /// <returns>已使用字节数</returns>
        int ProtocolProcess(byte[] buffer,int offset,int length,out IMsg requestInfo);
    }
}
