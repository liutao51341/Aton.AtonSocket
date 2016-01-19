using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Core.Handler
{
    /// <summary>
    /// 请求处理器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface  IRequestHandler
    {
        /// <summary>
        /// 处理器名称
        /// </summary>
        string HandlerName { get;   }
        /// <summary>
        /// 命令号 
        /// 空为全局命令
        /// </summary>
        string HandleCmdCode { get; }
        /// <summary>
        /// 处理器执行操作
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        bool ExecuteHandler(IRequestMsg requestInfo, ISocketSession session);
    }
}
