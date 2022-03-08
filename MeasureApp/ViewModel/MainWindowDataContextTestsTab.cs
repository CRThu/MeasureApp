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

        private string taskRunResultsConfigFilePath;
        public string TaskRunResultsConfigFilePath
        {
            get => taskRunResultsConfigFilePath;
            set
            {
                taskRunResultsConfigFilePath = value;
                RaisePropertyChanged(() => TaskRunResultsConfigFilePath);
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
                    string f = File.ReadAllText(@"D:\Projects\MeasureApp\MeasureApp\AutomationExamples\TaskRunClassDemo.cs");
                    Assembly assembly = CodeCompiler.Run(f);
                    Type t = assembly.GetTypes().First();

                    // RunTaskItemsCollection = new(ConvertClassToRunTaskItems(typeof(TaskRunClassDemo)));
                    RunTaskItemsCollection = new(ConvertClassToRunTaskItems(t));
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
                    InitialDirectory = Properties.Settings.Default.DefaultDirectory,
                    CheckFileExists = false
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    Properties.Settings.Default.DefaultDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                    TaskRunResultsConfigFilePath = openFileDialog.FileName;
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

        // 读取配置
        public void ReadResultsConfig()
        {
            try
            {
                string chipsTrimInfoJson = File.ReadAllText(TaskRunResultsConfigFilePath);
                TaskResultsStorage chipsTrimInfo = TaskResultsStorage.Deserialize(chipsTrimInfoJson);

                RunTaskItemsCollection.ToList().ForEach(i => i.ReturnVal = chipsTrimInfo.Get(TaskRunResultId.ToString(), i.Description));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // 配置更新到文件
        public void WriteResultsConfig()
        {
            try
            {
                // 若存在文件则加载
                TaskResultsStorage chipsTrimInfo = new();
                if (File.Exists(TaskRunResultsConfigFilePath))
                {
                    string json = File.ReadAllText(TaskRunResultsConfigFilePath);
                    chipsTrimInfo = TaskResultsStorage.Deserialize(json);
                }
                else
                {
                    SaveFileDialog saveFileDialog = new()
                    {
                        Title = "保存芯片Trim文件",
                        FileName = $"ChipsTrimInfo.{DataStorage.GenerateDateTimeNow()}.json",
                        DefaultExt = ".json",
                        Filter = "Json File|*.json",
                        InitialDirectory = Properties.Settings.Default.DefaultDirectory
                    };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        Properties.Settings.Default.DefaultDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                        TaskRunResultsConfigFilePath = saveFileDialog.FileName;
                    }
                    else
                        return;
                }

                RunTaskItemsCollection.ToList().ForEach(i => chipsTrimInfo.Set(TaskRunResultId.ToString(), i.Description, i.ReturnVal));

                string jsonObject = JsonConvert.SerializeObject(chipsTrimInfo, new JsonSerializerSettings() { FloatParseHandling = FloatParseHandling.Decimal });
                File.WriteAllText(TaskRunResultsConfigFilePath, jsonObject);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

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

        private CommandBase readResultsConfigEvent;
        public CommandBase ReadResultsConfigEvent
        {
            get
            {
                if (readResultsConfigEvent == null)
                {
                    readResultsConfigEvent = new CommandBase(new Action<object>(param => ReadResultsConfig()));
                }
                return readResultsConfigEvent;
            }
        }

        private CommandBase writeResultsConfigEvent;
        public CommandBase WriteResultsConfigEvent
        {
            get
            {
                if (writeResultsConfigEvent == null)
                {
                    writeResultsConfigEvent = new CommandBase(new Action<object>(param => WriteResultsConfig()));
                }
                return writeResultsConfigEvent;
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
