using MeasureApp.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // DataStorage数据源选择
        private string dataStorageSelectedValue;
        public string DataStorageSelectedValue
        {
            get => dataStorageSelectedValue;
            set
            {
                dataStorageSelectedValue = value;
                RaisePropertyChanged(() => DataStorageSelectedValue);
            }
        }

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

        // DataStorage添加键值对文本
        private string dataStorageAddKeyNameText = "New Key Name";
        public string DataStorageAddKeyNameText
        {
            get => dataStorageAddKeyNameText;
            set
            {
                dataStorageAddKeyNameText = value;
                RaisePropertyChanged(() => DataStorageAddKeyNameText);
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
                            if (DataStorageSelectedValue is null)
                                DataStorageSelectedValue = DataStorageInstance.DataStorageDictionary.Keys.FirstOrDefault();
                            if (DataStorageSelectedValue is null)
                                DataStorageDataGridBinding = null;
                            else
                                DataStorageDataGridBinding = DataStorageInstance.DataStorageDictionary[DataStorageSelectedValue];
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
                            string dataStorageKey = DataStorageSelectedValue;
                            SaveFileDialog saveFileDialog = new()
                            {
                                Title = "存储数据",
                                FileName = DataStorage.GenerateFileName(dataStorageKey),
                                DefaultExt = ".txt",
                                Filter = "Text File|*.txt"
                            };
                            if (saveFileDialog.ShowDialog() == true)
                            {
                                DataStorageInstance.Save(dataStorageKey, saveFileDialog.FileName);
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
                            if (MessageBox.Show("清理本次通信数据，是否继续？", "清理数据确认", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            {
                                DataStorageInstance.ClearAllData(DataStorageSelectedValue);
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

        // 数据源添加键值对
        private CommandBase dataStorageAddKeyValuePairEvent;
        public CommandBase DataStorageAddKeyValuePairEvent
        {
            get
            {
                if (dataStorageAddKeyValuePairEvent == null)
                {
                    dataStorageAddKeyValuePairEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            DataStorageInstance.AddKey(DataStorageAddKeyNameText);
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return dataStorageAddKeyValuePairEvent;
            }
        }

        // 数据源删除键值对
        private CommandBase dataStorageRemoveKeyValuePairEvent;
        public CommandBase DataStorageRemoveKeyValuePairEvent
        {
            get
            {
                if (dataStorageRemoveKeyValuePairEvent == null)
                {
                    dataStorageRemoveKeyValuePairEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            if (MessageBox.Show("删除键值对，是否继续？", "删除键值对确认", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            {
                                DataStorageInstance.RemoveKey(DataStorageSelectedValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return dataStorageRemoveKeyValuePairEvent;
            }
        }


        // 数据源加载
        private CommandBase dataStorageLoadEvent;
        public CommandBase DataStorageLoadEvent
        {
            get
            {
                if (dataStorageLoadEvent == null)
                {
                    dataStorageLoadEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            string json = File.ReadAllText(@"C:\Users\Administrator\Desktop\serialize.json");
                            var options = new JsonSerializerOptions
                            {
                                IncludeFields = true
                            };
                            DataStorage ds = JsonSerializer.Deserialize<DataStorage>(json, options);
                            // locker序列化与反序列化可能存在bug，需要编写新载入函数
                            DataStorageInstance = ds;
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return dataStorageLoadEvent;
            }
        }

        // 数据源保存
        private CommandBase dataStorageSaveEvent;
        public CommandBase DataStorageSaveEvent
        {
            get
            {
                if (dataStorageSaveEvent == null)
                {
                    dataStorageSaveEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            var options = new JsonSerializerOptions
                            {
                                IncludeFields = true
                            };
                            string json = JsonSerializer.Serialize(DataStorageInstance, options);
                            File.WriteAllText(@"C:\Users\Administrator\Desktop\serialize.json", json);
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return dataStorageSaveEvent;
            }
        }
    }
}
