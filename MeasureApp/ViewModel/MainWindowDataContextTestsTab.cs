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
using System.Collections.ObjectModel;
using MeasureApp.Model.DynamicCompilation;
using MeasureApp.Model.DataStorage;

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
                    Title = "Open File...",
                    Filter = "CSharp文件|*.cs|动态链接库|*.dll|所有文件|*.*",
                    InitialDirectory = Properties.Settings.Default.DefaultDirectory
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    Properties.Settings.Default.DefaultDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                    TaskRunConfigFilePath = openFileDialog.FileName;

                    Assembly assembly;
                    string ext = Path.GetExtension(openFileDialog.FileName);
                    if (ext == ".dll")
                    {
                        // Load
                        // string pdbPath = openFileDialog.FileName.Replace(".dll", ".pdb");

                        // assembly = CodeCompiler.Load(openFileDialog.FileName, File.Exists(pdbPath) ? pdbPath : null);
                        assembly = CodeCompiler.Load(openFileDialog.FileName);
                    }
                    else
                    {
                        // Compile
                        string f = File.ReadAllText(TaskRunConfigFilePath);
                        // VS调试时不能多次加载pdb文件否则会出现文件占用
                        //assembly = CodeCompiler.Run(f,
                        //    dllPath: openFileDialog.FileName.Replace(ext, ".dll"),
                        //    pdbPath: openFileDialog.FileName.Replace(ext, ".pdb"));
                        assembly = CodeCompiler.Run(f, dllPath: openFileDialog.FileName.Replace(ext, ".dll"));
                    }

                    Type t = assembly.GetTypes().First();

                    // RunTaskItemsCollection = new(RunTaskItem.ConvertClassToRunTaskItems(typeof(TaskRunClassDemo)));
                    RunTaskItemsCollection = new(RunTaskItem.ConvertClassToRunTaskItems(t));
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

                    var storage = TaskResultsStorage.Deserialize(File.ReadAllText(openFileDialog.FileName));
                    if (!storage.IsEqualCurrentVersion())
                    {
                        MessageBox.Show($"[WARNING]: Json文件版本或程序版本可能不兼容: " + Environment.NewLine +
                            $"存储类版本: {string.Join('.', storage.ClassVersion)} ( {string.Join('.', TaskResultsStorage.defaultClassVersion)} )" + Environment.NewLine +
                            $"Assembly版本: {string.Join('.', storage.AssemblyVersion)} ( {string.Join('.', TaskResultsStorage.GetAssemblyVersionArray())} )");
                    }

                    if (storage.IsAutoLoadAssemblyDll)
                    {
                        TaskRunConfigFilePath = storage.AssemblyDllPath;
                        if (File.Exists(TaskRunConfigFilePath))
                        {
                            Assembly assembly = CodeCompiler.Load(TaskRunConfigFilePath);
                            Type t = assembly.GetTypes().First();
                            RunTaskItemsCollection = new(RunTaskItem.ConvertClassToRunTaskItems(t));
                        }
                        else
                        {
                            throw new FileNotFoundException("未能加载程序集文件:" + TaskRunConfigFilePath);
                        }
                    }
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
                string json = File.ReadAllText(TaskRunResultsConfigFilePath);
                TaskResultsStorage storage = TaskResultsStorage.Deserialize(json);

                RunTaskItemsCollection.ToList().ForEach(i => (i.ParamVal, i.ReturnVal) = storage.Get(TaskRunResultId.ToString(), i.Description));
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
                TaskResultsStorage taskResultsStorage = new();
                if (File.Exists(TaskRunResultsConfigFilePath))
                {
                    string json = File.ReadAllText(TaskRunResultsConfigFilePath);
                    taskResultsStorage = TaskResultsStorage.Deserialize(json);
                }
                else
                {
                    SaveFileDialog saveFileDialog = new()
                    {
                        Title = "保存任务结果文件",
                        FileName = $"TaskResultsStorage.{DataStorage.GenerateDateTimeNow()}.json",
                        DefaultExt = ".json",
                        Filter = "Json File|*.json",
                        InitialDirectory = Properties.Settings.Default.DefaultDirectory
                    };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        Properties.Settings.Default.DefaultDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                        TaskRunResultsConfigFilePath = saveFileDialog.FileName;

                        string ext = Path.GetExtension(TaskRunConfigFilePath);
                        taskResultsStorage.IsAutoLoadAssemblyDll = true;
                        taskResultsStorage.AssemblyDllPath = TaskRunConfigFilePath.Replace(ext, ".dll");
                        RunTaskItemsCollection.ToList().ForEach(i => taskResultsStorage.Set(TaskRunResultId.ToString(), i.Description, (i.ParamVal, i.ReturnVal)));

                        string jsonObject = JsonConvert.SerializeObject(taskResultsStorage, new JsonSerializerSettings() { FloatParseHandling = FloatParseHandling.Decimal });
                        File.WriteAllText(TaskRunResultsConfigFilePath, jsonObject);
                    }
                }
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
