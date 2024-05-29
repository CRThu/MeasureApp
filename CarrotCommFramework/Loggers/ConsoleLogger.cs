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
        /// <param name="name">实例唯一名称</param>
        public ConsoleLogger(string name) : base(name)
        {

        }
    }
}
