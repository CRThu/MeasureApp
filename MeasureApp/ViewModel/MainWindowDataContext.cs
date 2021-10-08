using MeasureApp.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.ViewModel
{
    public class MainWindowDataContext : NotificationObjectBase
    {
        // 3458A 通信类
        public GPIB3458AMeasure measure3458A = new GPIB3458AMeasure();

        // 多串口通信类
        public SerialPorts serialPorts = new SerialPorts();

        // 数据存储
        public DataStorage dataStorage = new DataStorage();

        // 状态栏
        private string _statusBarText = "statusBar";
        public string StatusBarText
        {
            get => _statusBarText;
            set
            {
                _statusBarText = value;
                RaisePropertyChanged(() => StatusBarText);
            }
        }


    }
}
