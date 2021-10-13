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
        string Key3458AString = "3458A Data Storage";
        string KeySerialPortString = "Serial Port Data Storage";

        // 3458A 通信类
        private GPIB3458AMeasure measure3458AInstance = new();
        public GPIB3458AMeasure Measure3458AInstance
        {
            get => measure3458AInstance;
            set
            {
                measure3458AInstance = value;
                RaisePropertyChanged(() => Measure3458AInstance);
            }
        }

        // 多串口通信类
        private SerialPorts serialPortsInstance = new();
        public SerialPorts SerialPortsInstance
        {
            get => serialPortsInstance;
            set
            {
                serialPortsInstance = value;
                RaisePropertyChanged(() => SerialPortsInstance);
            }
        }

        // 数据存储类
        private DataStorage dataStorageInstance = new();
        public DataStorage DataStorageInstance
        {
            get => dataStorageInstance;
            set
            {
                dataStorageInstance = value;
                RaisePropertyChanged(() => DataStorageInstance);
            }
        }

        public MainWindowDataContext()
        {
            // 添加默认Key
            dataStorageInstance.AddKey(Key3458AString);
            dataStorageInstance.AddKey(KeySerialPortString);
        }


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

        // 通用DataGrid自动添加行号
        private CommandBase dataGridLoadingRowAddRowIndexEvent;
        public CommandBase DataGridLoadingRowAddRowIndexEvent
        {
            get
            {
                if (dataGridLoadingRowAddRowIndexEvent == null)
                {
                    dataGridLoadingRowAddRowIndexEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            if (param is DataGridRowEventArgs)
                            {
                                var row = (param as DataGridRowEventArgs).Row;
                                row.Header = (row.GetIndex() + 1).ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return dataGridLoadingRowAddRowIndexEvent;
            }
        }
    }
}
