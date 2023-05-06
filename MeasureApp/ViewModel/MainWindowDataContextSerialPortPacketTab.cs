using MeasureApp.Model.Common;
using MeasureApp.Model.Converter;
using MeasureApp.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 串口选择
        private string spPktCommTabSerialPortName;
        public string SpPktCommTabSerialPortName
        {
            get => spPktCommTabSerialPortName;
            set
            {
                spPktCommTabSerialPortName = value;
                RaisePropertyChanged(() => SpPktCommTabSerialPortName);
            }
        }

        public string[] SpPktCommTabProtocolList { get; } = new[] { "Ascii", "Data256", "Bin" };

        // 协议选择
        private string spPktCommTabProtocolName;
        public string SpPktCommTabProtocolName
        {
            get => spPktCommTabProtocolName;
            set
            {
                spPktCommTabProtocolName = value;
                RaisePropertyChanged(() => SpPktCommTabProtocolName);
            }
        }

        // 发送数据文本
        private string spPktCommTabTxText;
        public string SpPktCommTabTxText
        {
            get => spPktCommTabTxText;
            set
            {
                spPktCommTabTxText = value;
                RaisePropertyChanged(() => SpPktCommTabTxText);
            }
        }

        public void SpPktCommTabReadPktGUI(object param)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // CommandBase
        private CommandBase spPktCommTabReadPktEvent;
        public CommandBase SpPktCommTabReadPktEvent
        {
            get
            {
                if (spPktCommTabReadPktEvent == null)
                {
                    spPktCommTabReadPktEvent = new CommandBase(new Action<object>(param => SpPktCommTabReadPktGUI(param)));
                }
                return spPktCommTabReadPktEvent;
            }
        }

    }
}
