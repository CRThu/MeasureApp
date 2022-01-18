using MeasureApp.Model.Common;
using MeasureApp.ViewModel.Common;
using System;
using System.Collections.Generic;
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

        // 读取单字数据包长度
        private int spPktCommTabReadWordPktLen = 4;
        public int SpPktCommTabReadWordPktLen
        {
            get => spPktCommTabReadWordPktLen;
            set
            {
                spPktCommTabReadWordPktLen = value;
                RaisePropertyChanged(() => SpPktCommTabReadWordPktLen);
            }
        }

        // 读取单字数据包
        private void SpPktCommTabReadWordPkt(object param)
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
        private CommandBase spPktCommTabReadWordPktEvent;
        public CommandBase SpPktCommTabReadWordPktEvent
        {
            get
            {
                if (spPktCommTabReadWordPktEvent == null)
                {
                    spPktCommTabReadWordPktEvent = new CommandBase(new Action<object>(param => SpPktCommTabReadWordPkt(param)));
                }
                return spPktCommTabReadWordPktEvent;
            }
        }

    }
}
