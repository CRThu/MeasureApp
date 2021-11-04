using MeasureApp.Model;
using Microsoft.Win32;
using OxyPlot.Series;
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
                                DataStorageSelectedValue = dataStorageInstance.DataStorageDictionary.Keys.FirstOrDefault();
                            if (DataStorageSelectedValue is null)
                                DataStorageDataGridBinding = null;
                            else
                                DataStorageDataGridBinding = dataStorageInstance.DataStorageDictionary[DataStorageSelectedValue];

                            // TEST 未刷新
                            var lineSeries = plotViewPlotModel.Series[0] as LineSeries;
                            for (int x = 0; x < DataStorageDataGridBinding.Count; x++)
                            {
                                double y = Convert.ToDouble(DataStorageDataGridBinding[x].StringData);
                                lineSeries.Points.Add(new OxyPlot.DataPoint(x, y));
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
                                dataStorageInstance.Save(dataStorageKey, saveFileDialog.FileName);
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
                                dataStorageInstance.ClearAllData(DataStorageSelectedValue);
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
                            dataStorageInstance.AddKey(DataStorageAddKeyNameText);
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

        // 数据源添加键值对
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
                            dataStorageInstance.RemoveKey(DataStorageSelectedValue);
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
    }
}
