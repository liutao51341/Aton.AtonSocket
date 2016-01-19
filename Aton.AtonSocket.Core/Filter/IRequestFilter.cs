using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Core.Filter
{
    /// <summary>
    ///  请求数据过滤器
    /// </summary>
    public interface IRequestFilter
    {
        /// <summary>
        /// 过滤器名称
        /// </summary>
        string FilterName { get; }
        /// <summary>
        /// 优先级序号
        /// </summary>
        int IndexNo { get; }
        /// <summary>
        /// 过滤数据
        /// </summary>
        /// <param name="requestMsg"></param>
        /// <param name="socketSession"></param>
        /// <returns></returns>
        bool ProcessFilter(ref IRequestMsg requestMsg);
    }
}
