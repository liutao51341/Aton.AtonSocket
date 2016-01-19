using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Core.Exceptions
{
    /// <summary>
    /// 请求处理器异常
    /// </summary>
    public class MsgHandlerException : Exception
    {
        public string RequestHandlerName { get; set; }

        public MsgHandlerException(string message, string requestHandlerName)
            : base(message)
        {
            RequestHandlerName = requestHandlerName;
        }

        public MsgHandlerException(string message, string requestHandlerName, Exception innerException)
            : base(message, innerException)
        {
            RequestHandlerName = requestHandlerName;
        }
    }
}
