using MeasureApp.Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using LiveCharts.Geared;
using LiveCharts.Wpf;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Data;
using System.IO;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        public string Key3458AString = "3458A Data Storage";
        public string KeySerialPortString = "Serial Port Data Storage";

        public MainWindowDataContext()
        {
            // 添加默认Key
            DataStorageInstance.AddKey(Key3458AString);
            DataStorageInstance.AddKey(KeySerialPortString);
            DataStorageSelectedValue = Key3458AString;

            // TEST
            PlotViewLineValues.WithQuality(Quality.Highest);
            //PlotViewLineValues.CollectionChanged += (_, _) => MessageBox.Show($"{PlotViewLineValues.Count}");

            // async
            Application.Current.Dispatcher.Invoke(() =>
            {
                BindingOperations.EnableCollectionSynchronization(SerialportCommandLog, serialPortCommandLoglocker);
            });

            // 加载串口预设指令
            if (File.Exists(Properties.Settings.Default.DefaultPresetCommandsJsonPath))
            {
                SerialPortLoadPresetCommandsFromJson(Properties.Settings.Default.DefaultPresetCommandsJsonPath);
            }

            // 加载串口记录模块关键词颜色文件
            if (File.Exists(Properties.Settings.Default.DefaultLogKeywordColorJsonPath))
            {
                SerialPortCommLog.SerialPortLogLoadKeywordColorFromJson(Properties.Settings.Default.DefaultLogKeywordColorJsonPath);
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

        // MainWindow关闭事件
        private CommandBase mainWindowLoadedEvent;
        public CommandBase MainWindowLoadedEvent
        {
            get
            {
                if (mainWindowLoadedEvent == null)
                {
                    mainWindowLoadedEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            GpibDeviceSearchEvent.Execute(null);
                            SerialPortDeviceSearchEvent.Execute(null);
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return mainWindowLoadedEvent;
            }
        }

        // MainWindow关闭事件
        private CommandBase mainWindowClosedEvent;
        public CommandBase MainWindowClosedEvent
        {
            get
            {
                if (mainWindowClosedEvent == null)
                {
                    mainWindowClosedEvent = new CommandBase(new Action<object>(param =>
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
                    }));
                }
                return mainWindowClosedEvent;
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

        // 状态栏已用时间
        private int statusBarProgressElapsedTime = 0;
        public int StatusBarProgressElapsedTime
        {
            get => statusBarProgressElapsedTime;
            set
            {
                statusBarProgressElapsedTime = value;
                RaisePropertyChanged(() => StatusBarProgressElapsedTime);
            }
        }

        // 状态栏预计时间
        private int statusBarProgressETATime = 0;
        public int StatusBarProgressETATime
        {
            get => statusBarProgressETATime;
            set
            {
                statusBarProgressETATime = value;
                RaisePropertyChanged(() => StatusBarProgressETATime);
            }
        }

        //状态栏进度条
        private int statusBarProgressBarMin = 0;
        public int StatusBarProgressBarMin
        {
            get => statusBarProgressBarMin;
            set
            {
                statusBarProgressBarMin = value;
                RaisePropertyChanged(() => StatusBarProgressBarMin);
            }
        }

        private int statusBarProgressBarMax = 100;
        public int StatusBarProgressBarMax
        {
            get => statusBarProgressBarMax;
            set
            {
                statusBarProgressBarMax = value;
                RaisePropertyChanged(() => StatusBarProgressBarMax);
            }
        }

        private int statusBarProgressBarValue = 0;
        public int StatusBarProgressBarValue
        {
            get => statusBarProgressBarValue;
            set
            {
                statusBarProgressBarValue = value;
                RaisePropertyChanged(() => StatusBarProgressBarValue);
            }
        }

        // 进度显示函数
        public Stopwatch progressStopWatch = new();
        public void ProgressStopWatchStart(int count, int start = 0)
        {
            StatusBarProgressBarMin = start;
            StatusBarProgressBarMax = count - 1;
            progressStopWatch.Restart();
        }

        public void ProgressStopWatchStop()
        {
            progressStopWatch.Stop();
        }

        public void ProgressStopWatchUpdate(int value)
        {
            StatusBarProgressBarValue = value;
            StatusBarProgressElapsedTime = (int)((double)progressStopWatch.ElapsedMilliseconds / 1000);
            StatusBarProgressETATime = (int)(((double)progressStopWatch.ElapsedMilliseconds / (value + 1) * (StatusBarProgressBarMax + 1) - progressStopWatch.ElapsedMilliseconds) / 1000);
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
