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
                                      switch (dataStorageTag)
                                      {
                                          case "Multimeter":
                                              dataStorage.Save(Key3458AString, saveFileDialog.FileName);
                                              break;
                                          case "SerialPort":
                                              dataStorage.Save(KeySerialPortString, saveFileDialog.FileName);
                                              break;
                                          default:
                                              break;
                                      }
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
                                    switch (param as string)
                                    {
                                        case "Multimeter":
                                            dataStorage.ClearAllData(Key3458AString);
                                            break;
                                        case "SerialPort":
                                            dataStorage.ClearAllData(KeySerialPortString);
                                            break;
                                        default:
                                            break;
                                    }
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
    }
}
