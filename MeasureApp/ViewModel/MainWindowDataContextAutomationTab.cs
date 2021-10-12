using MeasureApp.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 临时发送DAC命令数据绑定
        private int loopTimesText = 262144;
        public int LoopTimesText
        {
            get => loopTimesText;
            set
            {
                loopTimesText = value;
                RaisePropertyChanged(() => LoopTimesText);
            }
        }

        private string sendCommandByteText = "02A600";
        public string SendCommandByteText
        {
            get => sendCommandByteText;
            set
            {
                sendCommandByteText = value;
                RaisePropertyChanged(() => SendCommandByteText);
            }
        }

        private decimal multiMeterSetRangeText = 10M;
        public decimal MultiMeterSetRangeText
        {
            get => multiMeterSetRangeText;
            set
            {
                multiMeterSetRangeText = value;
                RaisePropertyChanged(() => MultiMeterSetRangeText);
            }
        }

        private decimal multiMeterSetResolutionText = 1M;
        public decimal MultiMeterSetResolutionText
        {
            get => multiMeterSetResolutionText;
            set
            {
                multiMeterSetResolutionText = value;
                RaisePropertyChanged(() => MultiMeterSetResolutionText);
            }
        }

        private int delayText = 2;
        public int DelayText
        {
            get => delayText;
            set
            {
                delayText = value;
                RaisePropertyChanged(() => DelayText);
            }
        }
    }
}
