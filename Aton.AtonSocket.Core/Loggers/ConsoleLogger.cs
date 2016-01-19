using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.AtonSocket.Core.Loggers
{
    public class ConsoleLogger : ILogger
    {
        public ConsoleLogger(LoggerLevelE level) { m_LoggerLevel = level; }

        public override void ErrorLogger(string message, string exception)
        {
            if (m_LoggerLevel <= LoggerLevelE.ERROR)
            {
                Console.WriteLine("------------------------------------------------------------");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("Error ({0})--{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), message));
                Console.WriteLine(exception);
                Console.ResetColor();
            }

        }

        public override void InfoLogger(string message)
        {
            if (m_LoggerLevel <= LoggerLevelE.INFO)
            {
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine(string.Format("Info ({0})--{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), message));
            }
        }

        public override void WarnningLogger(string message)
        {
            if (m_LoggerLevel <= LoggerLevelE.WARNNING)
            {
                Console.WriteLine("------------------------------------------------------------");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(string.Format("Warnning ({0})--{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), message));
                Console.ResetColor();
            }
        }
    }
}
