using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Aton.AtonSocket.Core.Filter
{
    /// <summary>
    /// 连接过滤器
    /// </summary>
    public interface IConnectFilter
    {
        /// <summary>
        /// 过滤器名称
        /// </summary>
        string FilterName { get;  set; }
        /// <summary>
        /// 连接过滤
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        bool ConnectFilter(IPEndPoint RemoteEndPoint);
    }
}
