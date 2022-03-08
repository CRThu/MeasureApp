using ICSharpCode.AvalonEdit.Document;
using MeasureApp.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MeasureApp.Model.Common;
using MeasureApp.ViewModel.Common;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 自动化程序编辑器代码绑定
        private string automationCodeEditorText;
        public string AutomationCodeEditorText
        {
            get => automationCodeEditorText;
            set
            {
                automationCodeEditorText = value;
                RaisePropertyChanged(() => AutomationCodeEditorText);
            }
        }

        // 自动化模块加载脚本
        private CommandBase automationCodeLoadFileEvent;
        public CommandBase AutomationCodeLoadFileEvent
        {
            get
            {
                if (automationCodeLoadFileEvent == null)
                {
                    automationCodeLoadFileEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            // Open File Dialog
                            OpenFileDialog openFileDialog = new()
                            {
                                Title = "Open Script File...",
                                Filter = "C# Code|*.cs|Text File|*.txt",
                                InitialDirectory = Properties.Settings.Default.DefaultDirectory
                            };
                            if (openFileDialog.ShowDialog() == true)
                            {
                                Properties.Settings.Default.DefaultDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                                AutomationCodeEditorText = File.ReadAllText(openFileDialog.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return automationCodeLoadFileEvent;
            }
        }

        // 自动化模块保存脚本
        private CommandBase automationCodeSaveFileEvent;
        public CommandBase AutomationCodeSaveFileEvent
        {
            get
            {
                if (automationCodeSaveFileEvent == null)
                {
                    automationCodeSaveFileEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            // Save File Dialog
                            SaveFileDialog saveFileDialog = new()
                            {
                                Title = "Save Script File...",
                                Filter = "C# Code|*.cs|Text File|*.txt",
                                InitialDirectory = Properties.Settings.Default.DefaultDirectory
                            };
                            if (saveFileDialog.ShowDialog() == true)
                            {
                                Properties.Settings.Default.DefaultDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                                File.WriteAllText(saveFileDialog.FileName, AutomationCodeEditorText);
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return automationCodeSaveFileEvent;
            }
        }

        // 获取终止状态
        public bool IsAutomationCancelled => automationCancellationTokenSource.Token.IsCancellationRequested;


        // 3458A 同步电压显示开启标志位
        private bool automationIsRun = false;
        public bool AutomationIsRun

        {
            get => automationIsRun;
            set
            {
                automationIsRun = value;
                RaisePropertyChanged(() => AutomationIsRun);
            }
        }
        // 自动化模块执行事件
        private CancellationTokenSource automationCancellationTokenSource = new();
        private CommandBase automationCodeRunEvent;
        public CommandBase AutomationCodeRunEvent
        {
            get
            {
                if (automationCodeRunEvent == null)
                {
                    automationCodeRunEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            if (AutomationIsRun)
                            {
                                automationCancellationTokenSource.Cancel();
                            }
                            else
                            {
                                automationCancellationTokenSource = new();
                            }

                            _ = Task.Run(() =>
                            {
                                try
                                {
                                    AutomationIsRun = true;
                                    AutomationCodeReturnData = "Compiling...";
                                    var assembly = CodeCompiler.Run(AutomationCodeEditorText);
                                    var type = assembly.GetTypes().First(x => x.Name == "Automation");
                                    AutomationCodeReturnData = "Running...";
                                    var transformer = Activator.CreateInstance(type);
                                    var newContent = type.GetMethod("Main").Invoke(transformer, new object[] { this });
                                    AutomationCodeReturnData = $"Return: {newContent}";
                                    AutomationIsRun = false;
                                }
                                catch (Exception ex)
                                {
                                    _ = MessageBox.Show(ex.ToString());
                                    AutomationIsRun = false;
                                }
                            }, automationCancellationTokenSource.Token);
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return automationCodeRunEvent;
            }
        }

        // 自动化模块运行返回值
        private string automationCodeReturnData;
        public string AutomationCodeReturnData
        {
            get => automationCodeReturnData;
            set
            {
                automationCodeReturnData = value;
                RaisePropertyChanged(() => AutomationCodeReturnData);
            }
        }
    }
}
