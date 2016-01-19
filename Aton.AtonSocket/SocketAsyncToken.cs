using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket
{
    /// <summary>
    /// 通讯传递参数
    /// </summary>
    internal class SocketAsyncToken
    {
        /// <summary>
        /// 原始偏移量
        /// </summary>
        public int OriginOffset { get; internal set; }
        /// <summary>
        /// 当前偏移量
        /// </summary>
        public int OffsetDelta { get; set; }
        /// <summary>
        /// 剩余字节数
        /// </summary>
        public int LastLength { get; set; }
        /// <summary>
        /// 缓存区大小
        /// </summary>
        public int BufferSize { get; set; }

        public string TokenID { get; set; }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            OffsetDelta = OriginOffset;
            LastLength = 0;
            TokenID = string.Empty;
        }
    }
}
