using CarrotCommFramework.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Loggers
{
    /// <summary>
    /// 记录器接口
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 实例唯一名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 配置解析和初始化
        /// </summary>
        /// <param name="params"></param>
        public void Config(string[] @params = default!);

        /// <summary>
        /// 记录事件回调委托
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">数据包</param>
        public delegate void LogEventHandler(object sender, LogEventArgs e);

        /// <summary>
        /// 记录器回调方法
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">数据包</param>
        public void Log(object sender, LogEventArgs e);

        /// <summary>
        /// 获取历史数据包
        /// </summary>
        /// <param name="idx">消息记录索引</param>
        /// <param name="packet">数据包</param>
        /// <returns></returns>
        public bool TryGet(int idx, out Packet? packet);
    }

    public class LogEventArgs : EventArgs
    {
        public Packet? Packet { get; set; }
    }

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
        public LoggerBase()
        {
            Name = "__NAME__";
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
        public virtual bool TryGet(int idx, out Packet? packet)
        {
            // TODO
            packet = null;
            return false;
        }
    }
}
