using MeasureApp.Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Data;
using System.IO;
using MeasureApp.Model.Common;
using MeasureApp.ViewModel.Common;
using MeasureApp.Model.Devices;
using System.Linq;
using System.Collections.Specialized;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        public string Key3458AString = "3458A Data Storage";
        public string KeySerialPortString = "Serial Port Data Storage";

        // Pages
        private MainWindowPages pages;
        public MainWindowPages Pages
        {
            get => pages;
            set
            {
                pages = value;
                RaisePropertyChanged(() => Pages);
            }
        }


        public MainWindowDataContext()
        {
            Pages = new(this);

            // 添加默认Key
            DataStorageInstance.AddKey(Key3458AString);
            DataStorageInstance.AddKey(KeySerialPortString);
            DataStorageInstance.SelectedKey = Key3458AString;

            // 串口控制指令界面 寄存器表
            object SerialportCommandScriptVarDictLocker = new();
            BindingOperations.EnableCollectionSynchronization(SerialportCommandScriptVarDict, SerialportCommandScriptVarDictLocker);

            // 加载串口预设指令
            if (File.Exists(Properties.Settings.Default.DefaultPresetCommandsJsonPath))
            {
                SerialPortLoadPresetCommandsFromJson(Properties.Settings.Default.DefaultPresetCommandsJsonPath);
            }

            // 加载串口记录模块关键词颜色文件
            if (File.Exists(Properties.Settings.Default.DefaultLogKeywordColorJsonPath))
            {
                SerialPortLogger.LoadKeywordFile(Properties.Settings.Default.DefaultLogKeywordColorJsonPath);
            }
        }

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

        // 任务进度类
        private TaskProgress taskProgressInstance = new();
        public TaskProgress TaskProgressInstance
        {
            get => taskProgressInstance;
            set
            {
                taskProgressInstance = value;
                RaisePropertyChanged(() => TaskProgressInstance);
            }
        }


        // MainWindow加载事件
        public void MainWindowLoaded()
        {
            try
            {
                GpibSearchDevice();
                SerialPortSearchDevice();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // MainWindow关闭事件
        public void MainWindowClosed()
        {
            try
            {
                Measure3458AInstance.Dispose();
                SerialPortsInstance.CloseAll();
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // 状态栏文本
        private string statusBarText = "<NULL>";
        public string StatusBarText
        {
            get => statusBarText;
            set
            {
                statusBarText = value;
                RaisePropertyChanged(() => StatusBarText);
            }
        }

        // 通用DataGrid自动添加行号
        public static void DataGridLoadingRowAddRowIndex(object param)
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
        }

        public static void CopyToClipBoard(object param)
        {
            Clipboard.SetText(param.ToString());
        }

        // CommandBase
        private CommandBase mainWindowLoadedEvent;
        public CommandBase MainWindowLoadedEvent
        {
            get
            {
                if (mainWindowLoadedEvent == null)
                {
                    mainWindowLoadedEvent = new CommandBase(new Action<object>(param => MainWindowLoaded()));
                }
                return mainWindowLoadedEvent;
            }
        }

        private CommandBase mainWindowClosedEvent;
        public CommandBase MainWindowClosedEvent
        {
            get
            {
                if (mainWindowClosedEvent == null)
                {
                    mainWindowClosedEvent = new CommandBase(new Action<object>(param => MainWindowClosed()));
                }
                return mainWindowClosedEvent;
            }
        }

        private CommandBase dataGridLoadingRowAddRowIndexEvent;
        public CommandBase DataGridLoadingRowAddRowIndexEvent
        {
            get
            {
                if (dataGridLoadingRowAddRowIndexEvent == null)
                {
                    dataGridLoadingRowAddRowIndexEvent = new CommandBase(new Action<object>(param => DataGridLoadingRowAddRowIndex(param)));
                }
                return dataGridLoadingRowAddRowIndexEvent;
            }
        }

        private CommandBase copyToClipBoardEvent;
        public CommandBase CopyToClipBoardEvent
        {
            get
            {
                if (copyToClipBoardEvent == null)
                {
                    copyToClipBoardEvent = new CommandBase(new Action<object>(param => CopyToClipBoard(param)));
                }
                return copyToClipBoardEvent;
            }
        }
    }
}
