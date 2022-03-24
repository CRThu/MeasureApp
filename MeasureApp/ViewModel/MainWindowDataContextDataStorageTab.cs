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

        private string dataStorageNewKeyNameText = "New Key";
        /// <summary>
        /// 添加新键文本绑定
        /// </summary>
        public string DataStorageNewKeyNameText
        {
            get => dataStorageNewKeyNameText;
            set
            {
                dataStorageNewKeyNameText = value;
                RaisePropertyChanged(() => DataStorageNewKeyNameText);
            }
        }

        private bool dataStorageChartIsAutoRefresh = false;
        /// <summary>
        /// 图表自动刷新选项框绑定
        /// </summary>
        public bool DataStorageChartIsAutoRefresh
        {
            get => dataStorageChartIsAutoRefresh;
            set
            {
                dataStorageChartIsAutoRefresh = value;
                RaisePropertyChanged(() => DataStorageChartIsAutoRefresh);

                DataStorageChartRefreshEventRegister(DataStorageChartIsAutoRefresh);
            }
        }

        private int dataStorageChartAutoRefreshMinimumMilliSecond = 1000;
        /// <summary>
        /// 图表自动刷新最小触发时间绑定
        /// </summary>
        public int DataStorageChartAutoRefreshMinimumMilliSecond
        {
            get => dataStorageChartAutoRefreshMinimumMilliSecond;
            set
            {
                dataStorageChartAutoRefreshMinimumMilliSecond = value;
                RaisePropertyChanged(() => DataStorageChartAutoRefreshMinimumMilliSecond);
            }
        }

        /// <summary>
        /// 注册图表自动刷新事件
        /// </summary>
        /// <param name="isAutoRefresh"></param>
        public void DataStorageChartRefreshEventRegister(bool isAutoRefresh)
        {
            if (isAutoRefresh)
                DataStorageInstance.OnSelectedDataChanged += DataStorageChartRefreshEvent;
            else
                DataStorageInstance.OnSelectedDataChanged -= DataStorageChartRefreshEvent;
        }

        /// <summary>
        /// 上一次图表数据刷新时间
        /// </summary>
        public DateTime DataStorageChartLastRefreshTime = DateTime.Now;

        /// <summary>
        /// 图表数据刷新事件函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DataStorageChartRefreshEvent(object sender, EventArgs e)
        {
            if ((DateTime.Now - DataStorageChartLastRefreshTime).TotalMilliseconds >= DataStorageChartAutoRefreshMinimumMilliSecond)
            {
                // TODO
                Debug.WriteLine("Selected Data Changed.");
                DataStorageChartLastRefreshTime = DateTime.Now;
            }
            else
            {
                Debug.WriteLine("Busy, Not Triggered.");
            }
        }

        /// <summary>
        /// 数据源加载Json
        /// </summary>
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

        /// <summary>
        /// 数据源保存Json
        /// </summary>
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

        /// <summary>
        /// 选中数据导入
        /// </summary>
        public void DataStorageImportSelectedData()
        {
            try
            {
                // Open File Dialog
                OpenFileDialog openFileDialog = new()
                {
                    Title = "Open Text File...",
                    Filter = "Text File|*.txt",
                    InitialDirectory = Properties.Settings.Default.DefaultDirectory
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    Properties.Settings.Default.DefaultDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                    string[] lines = File.ReadAllLines(openFileDialog.FileName);
                    DataStorageInstance.AddValues(DataStorageInstance.SelectedKey, lines);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 选中数据导出
        /// </summary>
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

        /// <summary>
        /// 选中数据清空
        /// </summary>
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

        /// <summary>
        /// 添加键
        /// </summary>
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

        /// <summary>
        /// 删除键
        /// </summary>
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

        /// <summary>
        /// 添加测试点
        /// </summary>
        /// <param name="count"></param>
        /// <param name="isOneThread"></param>
        public void DataStorageAddTestValue(int count, bool isOneThread)
        {
            try
            {
                Task.Run(() =>
                {
                    Random random = new();
                    if (isOneThread)
                    {
                        double[] vs = new double[count];
                        for (int i = 0; i < vs.Length; i++)
                            vs[i] = i + random.NextDouble() - 0.5;
                        DataStorageInstance.AddValues(DataStorageInstance.SelectedKey, vs);
                    }
                    else
                    {
                        List<Task> tasksq = new();
                        for (int t = 0; t < 100 * DataStorageInstance.Keys.Length; t++)
                            tasksq.Add(new Task((x) =>
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    DataStorageInstance.AddValue(DataStorageInstance.Keys[(int)x], i + random.NextDouble() - 0.5);
                                    Task.Delay(1);
                                }
                            }
                            , (int)(t / 100.0)));
                        foreach (Task t in tasksq)
                            t.Start();
                    }
                });
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private CommandBase dataStorageChartManualRefreshEvent;
        public CommandBase DataStorageChartManualRefreshEvent
        {
            get
            {
                if (dataStorageChartManualRefreshEvent == null)
                {
                    dataStorageChartManualRefreshEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            var y = DataStorageInstance.SelectedData.Select(x => (double)x);
                            ECL.SetData(y);
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return dataStorageChartManualRefreshEvent;
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

        private CommandBase dataStorageImportSelectedDataEvent;
        public CommandBase DataStorageImportSelectedDataEvent
        {
            get
            {
                if (dataStorageImportSelectedDataEvent == null)
                {
                    dataStorageImportSelectedDataEvent = new CommandBase(new Action<object>(param => DataStorageImportSelectedData()));
                }
                return dataStorageImportSelectedDataEvent;
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

        private CommandBase dataStorageAddTestValueMultiThreadEvent;
        public CommandBase DataStorageAddTestValueMultiThreadEvent
        {
            get
            {
                if (dataStorageAddTestValueMultiThreadEvent == null)
                {
                    dataStorageAddTestValueMultiThreadEvent = new CommandBase(new Action<object>(param => DataStorageAddTestValue(Convert.ToInt32(param), false)));
                }
                return dataStorageAddTestValueMultiThreadEvent;
            }
        }
    }
}