using CarrotCommFramework.Factory;
using CarrotCommFramework.Loggers;
using CarrotCommFramework.Protocols;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolCommDemo.Logger
{
    public partial class DataLogger : ObservableLoggerBase
    {
        [ObservableProperty]
        private ObservableCollection<LogEventArgs> ds;

        public DataLogger() :
            base()
        {
            ds = [];
        }

        /// <summary>
        /// 记录器回调方法
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">数据包</param>
        public override void Log(object sender, LogEventArgs e)
        {
            Logger.Log(sender, e);
            Ds.Add(e);
        }

        /// <summary>
        /// 获取历史数据包
        /// </summary>
        /// <param name="idx">消息记录索引</param>
        /// <param name="packet">数据包</param>
        /// <returns></returns>
        public override bool TryGet(int idx, out Packet? packet)
        {
            if (idx < 0 || idx >= Ds.Count)
            {
                packet = null;
                return false;
            }
            else
            {
                packet = Ds[idx].Packet;
                return packet is not null;
            }
        }
    }
}
