using CarrotProtocolLib.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Streams
{
    /// <summary>
    /// 驱动层抽象通信流
    /// </summary>
    public interface IDriverCommStream : IStream
    {
        public string Address { get; set; }
        public string LoggerKey { get; set; }

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="addr"></param>
        public void Config(string? cfg);
    }
}
