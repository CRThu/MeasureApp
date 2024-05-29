using CarrotCommFramework.Loggers;
using CarrotCommFramework.Protocols;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolCommDemo.Logger
{
    public class ObservableLoggerBase : ObservableObject, ILogger
    {
        public LoggerBase Logger { get; set; }

        public ObservableLoggerBase(string name)
        {
            Logger = new LoggerBase(name);
        }

        public string Name
        {
            get
            {
                return Logger.Name;
            }
            set
            {
                SetProperty(Logger.Name, value, Logger, (u, n) => u.Name = n);
            }
        }

        /// <summary>
        /// 记录器配置方法
        /// </summary>
        /// <param name="params">配置参数</param>
        public virtual void Config(string[] @params = null)
        {
            Logger.Config(@params);
        }


        /// <summary>
        /// 记录器回调方法
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">数据包</param>
        public virtual void Log(object sender, LogEventArgs e)
        {
            Logger.Log(sender, e);
        }

        /// <summary>
        /// 获取历史数据包
        /// </summary>
        /// <param name="idx">消息记录索引</param>
        /// <param name="packet">数据包</param>
        /// <returns></returns>
        public virtual bool TryGet(int idx, out Packet? packet)
        {
            return Logger.TryGet(idx, out packet);
        }
    }
}
