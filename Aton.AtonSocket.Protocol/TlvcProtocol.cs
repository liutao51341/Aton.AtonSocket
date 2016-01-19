using Aton.AtonSocket.Core;
using Aton.AtonSocket.Protocol.Schemes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Protocol
{
    public class TlvcProtocol : IMsgProtocol
    {
        /// <summary>
        /// 协议解析处理
        /// </summary>
        /// <param name="buffer">缓存</param>
        /// <param name="offset">偏移量</param>
        /// <param name="length">长度</param>
        /// <param name="request">解析后对象</param>
        /// <returns>已使用字节数</returns>
        public int ProtocolProcess(byte[] buffer, int offset, int length, out IMsg requestInfo)
        {
            int usedBytes = 0;
            TlvcMessage msg = new TlvcMessage();
            if (length > 8)
            {
                int tag = BitConverter.ToInt32(buffer,offset);
                int len= BitConverter.ToInt32(buffer,offset+4);
                if (msg.Length >= length)
                {
                    msg.Tag = tag;
                    msg.Length = len;
                    msg.Data = new byte[msg.Length];
                    Array.Copy(buffer, offset+8, msg.Data, 0, msg.Length);
                    msg.Checksum = BitConverter.ToInt64(buffer,offset+8+msg.Length);
                    usedBytes = msg.Length + 16;
                }
            }
            requestInfo = msg;
            return usedBytes;
        }
    }
}
