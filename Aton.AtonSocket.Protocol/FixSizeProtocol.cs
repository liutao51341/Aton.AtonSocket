using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aton.AtonSocket.Core;
using Aton.AtonSocket.Protocol.Schemes;

namespace Aton.AtonSocket.Protocol
{
    public class FixSizeProtocol : IMsgProtocol
    {
        private int FixSize;

        public FixSizeProtocol(int fixSize) { FixSize = fixSize; }

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
            FixSizeMessage msg = new FixSizeMessage(FixSize);
            if (length >= FixSize)
            {
                Array.Copy(buffer, offset, msg.Data, 0, FixSize);
                usedBytes = FixSize;

            }
            requestInfo = msg;
            return usedBytes;
        }
    }
}
