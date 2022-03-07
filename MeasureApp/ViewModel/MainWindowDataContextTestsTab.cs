using MeasureApp.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MeasureApp.Model.Common;
using MeasureApp.ViewModel.Common;
using MeasureApp.Model.Register;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Threading;
using static MeasureApp.Model.RunTaskItem;
using System.Collections.ObjectModel;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 配置文件路径
        private string taskRunConfigFilePath;
        public string TaskRunConfigFilePath
        {
            get => taskRunConfigFilePath;
            set
            {
                taskRunConfigFilePath = value;
                RaisePropertyChanged(() => TaskRunConfigFilePath);
            }
        }

        private string taskRunChipConfigFilePath;
        public string TaskRunChipConfigFilePath
        {
            get => taskRunChipConfigFilePath;
            set
            {
                taskRunChipConfigFilePath = value;
                RaisePropertyChanged(() => TaskRunChipConfigFilePath);
            }
        }

        // 结果ID
        private int taskRunResultId;
        public int TaskRunResultId
        {
            get => taskRunResultId;
            set
            {
                taskRunResultId = value;
                RaisePropertyChanged(() => TaskRunResultId);
            }
        }

        // 函数实例
        private ObservableRangeCollection<RunTaskItem> runTaskItemsCollection = new();
        public ObservableRangeCollection<RunTaskItem> RunTaskItemsCollection
        {
            get => runTaskItemsCollection;
            set
            {
                runTaskItemsCollection = value;
                RaisePropertyChanged(() => RunTaskItemsCollection);
            }
        }

        public void LoadTaskItemsConfigFile()
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
                    TaskRunConfigFilePath = openFileDialog.FileName;

                    // Load
                    RunTaskItemsCollection = new(ConvertClassToRunTaskItems(typeof(TaskRunClassDemo)));
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        public void LoadTaskResultsConfigFile()
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
                    TaskRunChipConfigFilePath = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        public void RunAllTaskFunc()
        {
            try
            {
                Task.Run(() =>
                {
                    try
                    {
                        foreach (var runTaskItem in RunTaskItemsCollection)
                        {
                            Task t = runTaskItem.InvokeTaskFunc();
                            t.Wait();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void RunAllTaskResultProc()
        {
            try
            {
                Task.Run(() =>
                {
                    try
                    {
                        foreach (var runTaskItem in RunTaskItemsCollection)
                        {
                            Task t = runTaskItem.InvokeResultProcFunc();
                            t.Wait();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //// 读取配置
        //public void Nb2005LoadChipConfig()
        //{
        //    try
        //    {
        //        string chipsTrimInfoJson;
        //        ChipsTrimInfo chipsTrimInfo = new();
        //        chipsTrimInfoJson = File.ReadAllText(Nb2005ChipsConfigFilePath);
        //        chipsTrimInfo = JsonConvert.DeserializeObject<ChipsTrimInfo>(chipsTrimInfoJson, new JsonSerializerSettings() { FloatParseHandling = FloatParseHandling.Decimal });

        //        Nb2005TestTaskResult1 = chipsTrimInfo.Get(Nb2005ChipId, "TASK1");
        //        Nb2005TestTaskResult2 = chipsTrimInfo.Get(Nb2005ChipId, "TASK2");
        //        Nb2005TestTaskResult3 = chipsTrimInfo.Get(Nb2005ChipId, "TASK3");
        //        Nb2005TestTaskResult4 = chipsTrimInfo.Get(Nb2005ChipId, "TASK4");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //    }
        //}

        //// 配置更新到文件
        //public void Nb2005UpdateChipConfig()
        //{
        //    try
        //    {
        //        // 若存在文件则加载
        //        string chipsTrimInfoJson;
        //        ChipsTrimInfo chipsTrimInfo = new();
        //        if (File.Exists(Nb2005ChipsConfigFilePath))
        //        {
        //            chipsTrimInfoJson = File.ReadAllText(Nb2005ChipsConfigFilePath);
        //            chipsTrimInfo = JsonConvert.DeserializeObject<ChipsTrimInfo>(chipsTrimInfoJson, new JsonSerializerSettings() { FloatParseHandling = FloatParseHandling.Decimal });
        //        }
        //        else
        //        {
        //            SaveFileDialog saveFileDialog = new()
        //            {
        //                Title = "保存芯片Trim文件",
        //                FileName = $"ChipsTrimInfo.{DataStorage.GenerateDateTimeNow()}.json",
        //                DefaultExt = ".json",
        //                Filter = "Json File|*.json",
        //                InitialDirectory = Properties.Settings.Default.DefaultDirectory
        //            };
        //            if (saveFileDialog.ShowDialog() == true)
        //            {
        //                Properties.Settings.Default.DefaultDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
        //                Nb2005ChipsConfigFilePath = saveFileDialog.FileName;
        //            }
        //            else
        //                return;
        //        }

        //        chipsTrimInfo.Set(Nb2005ChipId, "TASK1", Nb2005TestTaskResult1);
        //        chipsTrimInfo.Set(Nb2005ChipId, "TASK2", Nb2005TestTaskResult2);
        //        chipsTrimInfo.Set(Nb2005ChipId, "TASK3", Nb2005TestTaskResult3);
        //        chipsTrimInfo.Set(Nb2005ChipId, "TASK4", Nb2005TestTaskResult4);

        //        chipsTrimInfoJson = JsonConvert.SerializeObject(chipsTrimInfo, new JsonSerializerSettings() { FloatParseHandling = FloatParseHandling.Decimal });
        //        File.WriteAllText(Nb2005ChipsConfigFilePath, chipsTrimInfoJson);
        //    }
        //    catch (Exception ex)
        //    {
        //        _ = MessageBox.Show(ex.ToString());
        //    }
        //}

        // CommandBase
        private CommandBase loadTaskItemsConfigFileEvent;
        public CommandBase LoadTaskItemsConfigFileEvent
        {
            get
            {
                if (loadTaskItemsConfigFileEvent == null)
                {
                    loadTaskItemsConfigFileEvent = new CommandBase(new Action<object>(param => LoadTaskItemsConfigFile()));
                }
                return loadTaskItemsConfigFileEvent;
            }
        }

        private CommandBase loadTaskResultsConfigFileEvent;
        public CommandBase LoadTaskResultsConfigFileEvent
        {
            get
            {
                if (loadTaskResultsConfigFileEvent == null)
                {
                    loadTaskResultsConfigFileEvent = new CommandBase(new Action<object>(param => LoadTaskResultsConfigFile()));
                }
                return loadTaskResultsConfigFileEvent;
            }
        }

        private CommandBase runAllTaskFuncEvent;
        public CommandBase RunAllTaskFuncEvent
        {
            get
            {
                if (runAllTaskFuncEvent == null)
                {
                    runAllTaskFuncEvent = new CommandBase(new Action<object>(param => RunAllTaskFunc()));
                }
                return runAllTaskFuncEvent;
            }
        }

        private CommandBase runAllTaskResultProcEvent;
        public CommandBase RunAllTaskResultProcEvent
        {
            get
            {
                if (runAllTaskResultProcEvent == null)
                {
                    runAllTaskResultProcEvent = new CommandBase(new Action<object>(param => RunAllTaskResultProc()));
                }
                return runAllTaskResultProcEvent;
            }
        }
    }

}
