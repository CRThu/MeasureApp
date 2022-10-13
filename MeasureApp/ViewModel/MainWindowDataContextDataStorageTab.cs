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
using System.Threading;
using MeasureApp.Model.DataStorage;

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

        private bool dataStorageChartIsAutoRefresh = true;
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
            }
        }

        private int dataStorageChartAutoRefreshMinimumMilliSecond = 500;
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
        /// 数据源复制到剪切板(可直接粘贴至Excel)
        /// NAME1       NAME2
        /// X   Y       X   Y
        /// 1   1111    1   123
        /// 2   2222    2   234
        /// 3   3333    3   345
        /// 4   4444    4   456
        /// 5   5555    5   567
        /// </summary>
        private void DataStorageCopyToClipBoard()
        {
            try
            {
                int rowsCnt = DataStorageInstance.Data.Select(kvp => kvp.Value.Count).Max();
                StringBuilder[] tableRows = new StringBuilder[rowsCnt + 2];
                for (int i = 0; i < tableRows.Length; i++)
                    tableRows[i] = new StringBuilder();
                foreach (var dataElement in DataStorageInstance.Data)
                {
                    tableRows[0].Append($"{dataElement.Key}\t\t\t");
                    tableRows[1].Append("X\tY\t\t");
                    for (int i = 0; i < dataElement.Value.DataPoints.Count; i++)
                    {
                        tableRows[i + 2].Append($"{dataElement.Value.DataPoints[i].X}\t{dataElement.Value.DataPoints[i].Y}\t\t");
                    }
                }
                Clipboard.SetText(string.Join("\r\n", tableRows.Select(row => row.ToString())));
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
                        for (int t = 0; t < 10 * DataStorageInstance.Keys.Length; t++)
                            tasksq.Add(new Task((x) =>
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    DataStorageInstance.AddValue(DataStorageInstance.Keys[(int)x], i + random.NextDouble() - 0.5);
                                    Thread.Sleep(100);
                                }
                            }
                            , (int)(t / 10.0)));
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

        public Action UpdateChartAction { get; set; }

        public void UpdateChart()
        {
            UpdateChartAction.Invoke();
        }

        public void DataStorageChartManualRefresh()
        {
            UpdateChart();
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

        private CommandBase dataStorageCopyToClipBoardEvent;
        public CommandBase DataStorageCopyToClipBoardEvent
        {
            get
            {
                if (dataStorageCopyToClipBoardEvent == null)
                {
                    dataStorageCopyToClipBoardEvent = new CommandBase(new Action<object>(param => DataStorageCopyToClipBoard()));
                }
                return dataStorageCopyToClipBoardEvent;
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

        private CommandBase dataStorageChartManualRefreshEvent;
        public CommandBase DataStorageChartManualRefreshEvent
        {
            get
            {
                if (dataStorageChartManualRefreshEvent == null)
                {
                    dataStorageChartManualRefreshEvent = new CommandBase(new Action<object>(param => DataStorageChartManualRefresh()));
                }
                return dataStorageChartManualRefreshEvent;
            }
        }

    }
}