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

        // 添加新键文本绑定
        private string dataStorageNewKeyNameText = "New Key";
        public string DataStorageNewKeyNameText
        {
            get => dataStorageNewKeyNameText;
            set
            {
                dataStorageNewKeyNameText = value;
                RaisePropertyChanged(() => DataStorageNewKeyNameText);
            }
        }

        // 加载Json
        public void DataStorageLoadJson()
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
                    DataStorageInstance = DataStorage.DeSerialize(json);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // 保存Json
        public void DataStorageSaveJson()
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
                    string json = DataStorage.Serialize(DataStorageInstance);
                    File.WriteAllText(saveFileDialog.FileName, json);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // 导出选中数据
        public void DataStorageExportSelectedData()
        {
            try
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Title = "存储数据",
                    FileName = DataStorage.GenerateFileName(DataStorageInstance.SelectedKey),
                    DefaultExt = ".txt",
                    Filter = "Text File|*.txt",
                    InitialDirectory = Properties.Settings.Default.DefaultDirectory
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    Properties.Settings.Default.DefaultDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                    File.WriteAllLines(saveFileDialog.FileName, DataStorageInstance.GetValues<string>(DataStorageInstance.SelectedKey));
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // 清空选中数据
        public void DataStorageClearSelectedData()
        {
            try
            {
                if (MessageBox.Show($"清理键:{DataStorageInstance.SelectedKey}的数据，是否继续？", "清理数据确认", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    DataStorageInstance.ClearValues(DataStorageInstance.SelectedKey);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // 添加键
        public void DataStorageAddKey()
        {
            try
            {
                DataStorageInstance.AddKey(DataStorageNewKeyNameText);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // 删除键
        public void DataStorageRemoveKey()
        {
            try
            {
                if (MessageBox.Show($"删除键:{DataStorageInstance.SelectedKey}, 是否继续？", "删除键确认", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    DataStorageInstance.RemoveKey(DataStorageInstance.SelectedKey);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // For Test
        public void DataStorageAddTestValue(int count, bool isOnce)
        {
            try
            {
                Task.Run(() =>
                {
                    double[] vs = new double[count];
                    Random random = new();
                    for (int i = 0; i < vs.Length; i++)
                        vs[i] = i + random.NextDouble() - 0.5;

                    if (isOnce)
                    {
                        DataStorageInstance.AddValues(DataStorageInstance.SelectedKey, vs);
                    }
                    else
                    {
                        int times = 0;
                        Stopwatch sw = new Stopwatch();
                        double intervalMilliSecond = 1000.0 / count;

                        sw.Start();
                        while (true)
                            if (sw.ElapsedMilliseconds > (times + 1) * intervalMilliSecond)
                            {
                                DataStorageInstance.AddValue(DataStorageInstance.SelectedKey, vs[times]);
                                times++;
                                if (times >= count)
                                {

                                    sw.Stop();
                                    Debug.WriteLine(sw.ElapsedMilliseconds);
                                }
                            }
                    }
                });
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private CommandBase dataStorageManualChartRefreshEvent;
        public CommandBase DataStorageManualChartRefreshEvent
        {
            get
            {
                if (dataStorageManualChartRefreshEvent == null)
                {
                    dataStorageManualChartRefreshEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            DataStorageInstance_OnDataChangedEvent(null, null);
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return dataStorageManualChartRefreshEvent;
            }
        }

        // CommandBase
        private CommandBase dataStorageLoadJsonEvent;
        public CommandBase DataStorageLoadJsonEvent
        {
            get
            {
                if (dataStorageLoadJsonEvent == null)
                {
                    dataStorageLoadJsonEvent = new CommandBase(new Action<object>(param => DataStorageLoadJson()));
                }
                return dataStorageLoadJsonEvent;
            }
        }

        private CommandBase dataStorageSaveJsonEvent;
        public CommandBase DataStorageSaveJsonEvent
        {
            get
            {
                if (dataStorageSaveJsonEvent == null)
                {
                    dataStorageSaveJsonEvent = new CommandBase(new Action<object>(param => DataStorageSaveJson()));
                }
                return dataStorageSaveJsonEvent;
            }
        }

        private CommandBase dataStorageExportSelectedDataEvent;
        public CommandBase DataStorageExportSelectedDataEvent
        {
            get
            {
                if (dataStorageExportSelectedDataEvent == null)
                {
                    dataStorageExportSelectedDataEvent = new CommandBase(new Action<object>(param => DataStorageExportSelectedData()));
                }
                return dataStorageExportSelectedDataEvent;
            }
        }

        private CommandBase dataStorageClearSelectedDataEvent;
        public CommandBase DataStorageClearSelectedDataEvent
        {
            get
            {
                if (dataStorageClearSelectedDataEvent == null)
                {
                    dataStorageClearSelectedDataEvent = new CommandBase(new Action<object>(param => DataStorageClearSelectedData()));
                }
                return dataStorageClearSelectedDataEvent;
            }
        }

        private CommandBase dataStorageAddKeyEvent;
        public CommandBase DataStorageAddKeyEvent
        {
            get
            {
                if (dataStorageAddKeyEvent == null)
                {
                    dataStorageAddKeyEvent = new CommandBase(new Action<object>(param => DataStorageAddKey()));
                }
                return dataStorageAddKeyEvent;
            }
        }

        private CommandBase dataStorageRemoveKeyEvent;
        public CommandBase DataStorageRemoveKeyEvent
        {
            get
            {
                if (dataStorageRemoveKeyEvent == null)
                {
                    dataStorageRemoveKeyEvent = new CommandBase(new Action<object>(param => DataStorageRemoveKey()));
                }
                return dataStorageRemoveKeyEvent;
            }
        }

        // For Test
        private CommandBase dataStorageAddTestValueOnceEvent;
        public CommandBase DataStorageAddTestValueOnceEvent
        {
            get
            {
                if (dataStorageAddTestValueOnceEvent == null)
                {
                    dataStorageAddTestValueOnceEvent = new CommandBase(new Action<object>(param => DataStorageAddTestValue(Convert.ToInt32(param), true)));
                }
                return dataStorageAddTestValueOnceEvent;
            }
        }

        private CommandBase dataStorageAddTestValueIntervalEvent;
        public CommandBase DataStorageAddTestValueIntervalEvent
        {
            get
            {
                if (dataStorageAddTestValueIntervalEvent == null)
                {
                    dataStorageAddTestValueIntervalEvent = new CommandBase(new Action<object>(param => DataStorageAddTestValue(Convert.ToInt32(param), false)));
                }
                return dataStorageAddTestValueIntervalEvent;
            }
        }
    }
}