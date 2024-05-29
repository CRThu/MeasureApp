using CarrotCommFramework.Loggers;
using CarrotCommFramework.Protocols;
using CarrotCommFramework.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Services
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
    public interface ISessionServiceBase : IServiceBase<object>
    {

        /// <summary>
        /// 记录器事件
        /// </summary>
        public event LogEventHandler? Logging;

        /// <summary>
        /// 配置绑定
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="protocol"></param>
        public void Bind(IAsyncStream? stream, IProtocol? protocol = null);
    }

    /// <summary>
    /// 通信协议会话服务基类
    /// </summary>
    public abstract class SessionServiceBase : ServiceBase<object>, ISessionServiceBase
    {
        /// <summary>
        /// 数据流接口
        /// </summary>
        public IAsyncStream? Stream { get; set; }

        /// <summary>
        /// 数据协议接口
        /// </summary>
        public IProtocol? Protocol { get; set; }


        /// <summary>
        /// 记录器事件
        /// </summary>
        public event LogEventHandler? Logging;

        /// <summary>
        /// 配置绑定
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="protocol"></param>
        public virtual void Bind(IAsyncStream? stream, IProtocol? protocol = null)
        {
            Stream = stream;
            Protocol = protocol;
        }

        /// <summary>
        /// 服务实现方法
        /// </summary>
        /// <param name="ct">返回</param>
        /// <returns>返回值</returns>
        public override async Task<object?> Impl(CancellationToken ct)
        {
            await Impl();
            return await Task.FromResult<object?>(null);
        }


        /// <summary>
        /// 无返回值服务实现方法
        /// </summary>
        /// <returns></returns>
        public virtual async Task Impl()
        {
            await Task.Delay(1);
        }

        /// <summary>
        /// 引发基类事件，用于派生类重载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnServiceLogging(object sender, LogEventArgs e)
        {
            Logging?.Invoke(sender, e);
        }
    }
}
