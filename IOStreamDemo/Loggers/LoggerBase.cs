using IOStreamDemo.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Loggers
{
    /// <summary>
    /// 记录器基础类
    /// </summary>
    public class LoggerBase : ILogger
    {
        /// <summary>
        /// 实例唯一名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">实例唯一名称</param>
        public LoggerBase(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 记录器配置方法
        /// </summary>
        /// <param name="params">配置参数</param>
        public virtual void Config(string[] @params = null)
        {

        }

        /// <summary>
        /// 记录器回调方法
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">数据包</param>
        public virtual void Log(object sender, LogEventArgs e)
        {
            Console.WriteLine($"{GetType().FullName}: " + e.Packet.Message);
        }

        /// <summary>
        /// 获取历史数据包
        /// </summary>
        /// <param name="idx">消息记录索引</param>
        /// <param name="packet">数据包</param>
        /// <returns></returns>
        public bool TryGet(int idx, out Packet? packet)
        {
            // TODO
            packet = null;
            return false;
        }
    }
}
