using Aton.AtonSocket.Core.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Core
{
    public abstract class ILogger
    {
        
        public LoggerLevelE m_LoggerLevel { get; protected set; }

        public abstract void InfoLogger(string message);

        public abstract void WarnningLogger(string message);

        public abstract void ErrorLogger(string message, string exception);
    }
}
