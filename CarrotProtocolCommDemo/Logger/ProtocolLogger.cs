using CarrotProtocolLib.Logger;
using CarrotProtocolLib.Protocol;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolCommDemo.Logger
{
    public partial class ProtocolLogger : ObservableObject, ILogger
    {
        /// <summary>
        /// 协议存储列表
        /// </summary>
        [ObservableProperty]
        private List<IRecord> protocolList;

        [ObservableProperty]
        private DataLogger dataLogger;

        public delegate void RecordUpdateHandler(IRecord log);
        public event RecordUpdateHandler RecordUpdate;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ProtocolLogger()
        {
            ProtocolList = new();
            dataLogger = new();
            // 事件订阅
            RecordUpdate += DataLogger.Add;
        }

        /// <summary>
        /// 新增协议记录
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="frame"></param>
        public void Add(string from, string to, IProtocolFrame frame)
        {
            IRecord record = frame.ToRecord(from, to);
            ProtocolList.Add(record);
            RecordUpdate?.Invoke(record);
        }
    }
}
