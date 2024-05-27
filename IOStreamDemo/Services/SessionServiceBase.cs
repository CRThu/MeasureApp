using IOStreamDemo.Loggers;
using IOStreamDemo.Protocols;
using IOStreamDemo.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Services
{
    /// <summary>
    /// 记录器事件委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void LogEventHandler(object sender, LogEventArgs e);

    /// <summary>
    /// 通信协议会话服务接口
    /// </summary>
    public interface ISessionServiceBase<TReturn> : IServiceBase<TReturn>
    {

        /// <summary>
        /// 记录器事件
        /// </summary>
        public event LogEventHandler? Logging;


        public void Bind(IStream? stream, IProtocol? protocol = null);
    }

    /// <summary>
    /// 通信协议会话服务基类
    /// </summary>
    public abstract class SessionServiceBase : ServiceBase<object>, ISessionServiceBase<object>
    {
        /// <summary>
        /// 数据流接口
        /// </summary>
        public IStream? Stream { get; set; }

        /// <summary>
        /// 数据协议接口
        /// </summary>
        public IProtocol? Protocol { get; set; }


        /// <summary>
        /// 记录器事件
        /// </summary>
        public event LogEventHandler? Logging;

        public virtual void Bind(IStream? stream, IProtocol? protocol = null)
        {
            Stream = stream;
            Protocol = protocol;
        }
    }
}
