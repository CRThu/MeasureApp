using CarrotProtocolLib.Protocol;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Logger
{
    public partial class ProtocolLogger : ObservableObject, ILogger
    {
        /// <summary>
        /// 协议存储列表
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<IRecord> protocolList;

        // public delegate void RecordUpdateHandler(IRecord log);
        public event ILogger.RecordUpdateHandler RecordUpdate;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ProtocolLogger()
        {
            ProtocolList = new();
        }

        /// <summary>
        /// 新增协议记录
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="frame"></param>
        public void Add(string from, string to, IProtocolFrame frame)
        {
            ProtocolRecord record = new()
            {
                Time = DateTime.Now,
                From = from,
                To = to,
                Protocol = "<NULL>",
                Type = TransferType.Command,
                Frame = frame
            };
            ProtocolList.Add(record);
            RecordUpdate?.Invoke(record);
        }
    }
}
