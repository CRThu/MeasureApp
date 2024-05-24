using IOStreamDemo.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Loggers
{
    public class NLogLogger : LoggerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">实例唯一名称</param>
        public NLogLogger(string name) : base(name)
        {

        }
    }
}
