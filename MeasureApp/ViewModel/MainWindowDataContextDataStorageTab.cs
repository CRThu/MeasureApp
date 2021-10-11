using MeasureApp.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // DataGrid数据绑定
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

        // ListBox选中数据类后更新DataGrid事件
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
                            if (param is string)
                            {
                                string dataStorageKey = param as string;
                                DataStorageDataGridBinding = dataStorageInstance.DataStorageDictionary[dataStorageKey];
                            }
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
                                string dataStorageKey = param as string;
                                SaveFileDialog saveFileDialog = new()
                                {
                                    Title = "存储数据",
                                    FileName = DataStorage.GenerateFileName(dataStorageKey),
                                    DefaultExt = ".txt",
                                    Filter = "Text File|*.txt"
                                };
                                if (saveFileDialog.ShowDialog() == true)
                                {
                                    dataStorageInstance.Save(dataStorageKey, saveFileDialog.FileName);
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
                                    dataStorageInstance.ClearAllData(param as string);
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
