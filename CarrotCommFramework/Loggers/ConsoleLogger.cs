using CarrotCommFramework.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Loggers
{
    public class ConsoleLogger : LoggerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConsoleLogger() : base()
        {
        }
        public override void Log(object sender, LogEventArgs e)
        {
            Console.WriteLine($"{GetType().FullName}: " + e.Packet.Message);
        }
    }
}