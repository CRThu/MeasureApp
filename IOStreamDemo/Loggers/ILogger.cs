using IOStreamDemo.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Loggers
{
    public interface ILogger
    {
        public string Name { get; set; }

        /// <summary>
        /// 配置解析和初始化
        /// </summary>
        /// <param name="params"></param>
        public void Config(string[] @params = default!);
        public void Log(Packet message);
    }
}
