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
    public class MainWindowDataContext : NotificationObjectBase
    {
        // 重构临时变量
        string Key3458AString = "3458A Data Storage";
        string KeySerialPortString = "Serial Port Data Storage";

        // 3458A 通信类
        public GPIB3458AMeasure measure3458A = new();

        // 多串口通信类
        public SerialPorts serialPorts = new();

        // 数据存储
        public DataStorage dataStorage = new();

        // 数据存储页数据绑定
        private dynamic dataStorageDataGridBinding;
        public dynamic DataStorageDataGridBinding
        {
            get => dataStorageDataGridBinding;
            set
            {
                dataStorageDataGridBinding = value;
                RaisePropertyChanged(() => DataStorageDataGridBinding);
            }
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

        // 数据存储事件
        private CommandBase dataStorageStoreEvent;
        public CommandBase DataStorageStoreEvent
        {
            get
            {
                if (dataStorageStoreEvent == null)
                {
                    dataStorageStoreEvent = new CommandBase(new Action<object>(param =>
                      {
                          try
                          {
                              if (param is string)
                              {
                                  string dataStorageTag = param as string;
                                  SaveFileDialog saveFileDialog = new()
                                  {
                                      Title = "存储数据",
                                      FileName = DataStorage.GenerateFileName(dataStorageTag),
                                      DefaultExt = ".txt",
                                      Filter = "Text File|*.txt"
                                  };
                                  if (saveFileDialog.ShowDialog() == true)
                                  {
                                      dataStorage.Save(dataStorageTag, saveFileDialog.FileName);
                                  }
                              }
                          }
                          catch (Exception ex)
                          {
                              _ = MessageBox.Show(ex.ToString());
                          }
                      }));
                }
                return dataStorageStoreEvent;
            }
        }

        // 数据删除事件
        private CommandBase dataStorageDeleteEvent;
        public CommandBase DataStorageDeleteEvent
        {
            get
            {
                if (dataStorageDeleteEvent == null)
                {
                    dataStorageDeleteEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            if (param is string)
                            {
                                if (MessageBox.Show("清理本次通信数据，是否继续？", "清理数据确认", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                {
                                    dataStorage.ClearAllData(param as string);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return dataStorageDeleteEvent;
            }
        }

        // 数据显示触发事件
        private CommandBase dataStorageSelectionChangedEvent;
        public CommandBase DataStorageSelectionChangedEvent
        {
            get
            {
                if (dataStorageSelectionChangedEvent == null)
                {
                    dataStorageSelectionChangedEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            // TODO BIND变量替代路由args
                            string key = ((param as SelectionChangedEventArgs).AddedItems[0] as ListBoxItem).Tag as string;
                            DataStorageDataGridBinding = dataStorage.DataStorageDictionary[key];
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return dataStorageSelectionChangedEvent;
            }
        }

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

        private string sendCommandByteText = "A0";
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

        private int delayText = 100;
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
