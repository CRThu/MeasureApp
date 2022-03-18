using MeasureApp.Model;
using Microsoft.Win32;
using Newtonsoft.Json;
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
using MeasureApp.Model.Common;
using MeasureApp.ViewModel.Common;

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
                DataStorageSelectionChangedEvent.Execute(null);
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

        private EChartsLineData ecl = new();
        public EChartsLineData ECL
        {
            get => ecl;
            set
            {
                ecl = value;
                RaisePropertyChanged(() => ECL);
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
                            {
                                ECL.ClearData();
                                DataStorageDataGridBinding = null;
                            }
                            else
                            {
                                DataStorageDataGridBinding = DataStorageInstance.Dict[DataStorageSelectedValue];
                                var vals = DataStorageInstance[DataStorageSelectedValue];
                                double[] valsDouble = vals.Select(d => (double)d).ToArray();
                                double[] x = Enumerable.Range(0, valsDouble.Length).Select(xn => (double)xn).ToArray();
                                ECL.ClearData();
                                ECL.AddData(x, valsDouble);
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
                                Filter = "Text File|*.txt",
                                InitialDirectory = Properties.Settings.Default.DefaultDirectory
                            };
                            if (saveFileDialog.ShowDialog() == true)
                            {
                                Properties.Settings.Default.DefaultDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
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
                                DataStorageInstance.ClearDataCollection(DataStorageSelectedValue);
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
                            // Open File Dialog
                            OpenFileDialog openFileDialog = new()
                            {
                                Title = "Open Json File...",
                                Filter = "Json File|*.json",
                                InitialDirectory = Properties.Settings.Default.DefaultDirectory
                            };
                            if (openFileDialog.ShowDialog() == true)
                            {
                                Properties.Settings.Default.DefaultDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                                string json = File.ReadAllText(openFileDialog.FileName);
                                //var options = new JsonSerializerOptions
                                //{
                                //    IncludeFields = true
                                //};
                                //DataStorage ds = System.Text.Json.JsonSerializer.Deserialize<DataStorage>(json, options);
                                var ds = JsonConvert.DeserializeObject<DataStorage>(json, new JsonSerializerSettings() { FloatParseHandling = FloatParseHandling.Decimal });
                                DataStorageInstance.Load(ds);
                            }
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
                            SaveFileDialog saveFileDialog = new()
                            {
                                Title = "存储数据",
                                FileName = $"DataStorage.{DataStorage.GenerateDateTimeNow()}.json",
                                DefaultExt = ".json",
                                Filter = "Json File|*.json",
                                InitialDirectory = Properties.Settings.Default.DefaultDirectory
                            };
                            if (saveFileDialog.ShowDialog() == true)
                            {
                                Properties.Settings.Default.DefaultDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                                //var options = new JsonSerializerOptions
                                //{
                                //    IncludeFields = true
                                //};
                                //string json = System.Text.Json.JsonSerializer.Serialize(DataStorageInstance, options);
                                string json = JsonConvert.SerializeObject(DataStorageInstance, new JsonSerializerSettings() { FloatParseHandling = FloatParseHandling.Decimal });
                                File.WriteAllText(saveFileDialog.FileName, json);
                            }
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

        // 
        private CommandBase dataStorageAddTestEvent;
        public CommandBase DataStorageAddTestEvent
        {
            get
            {
                if (dataStorageAddTestEvent == null)
                {
                    dataStorageAddTestEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            int n = Convert.ToInt32(param);
                            dynamic[] vs = new dynamic[n];
                            Random random = new();
                            for (int i = 0; i < vs.Length; i++)
                                vs[i] = i + random.NextDouble() - 0.5;
                            DataStorageInstance.AddDataCollection(Key3458AString, vs.ToList());
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return dataStorageAddTestEvent;
            }
        }
    }
}