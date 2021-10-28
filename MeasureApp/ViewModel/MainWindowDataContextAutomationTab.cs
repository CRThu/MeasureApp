using ICSharpCode.AvalonEdit.Document;
using MeasureApp.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
                                InitialDirectory = Directory.GetCurrentDirectory()
                            };
                            if (openFileDialog.ShowDialog() == true)
                            {
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
                                InitialDirectory = Directory.GetCurrentDirectory()
                            };
                            if (saveFileDialog.ShowDialog() == true)
                            {
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

        // 自动化模块执行事件
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
                            _ = Task.Run(() =>
                            {
                                try
                                {
                                    string code = AutomationCodeEditorText;
                                    var type = CodeCompiler.Run(code, "Automation");
                                    var transformer = Activator.CreateInstance(type);
                                    var newContent = type.GetMethod("Main").Invoke(transformer, new object[] { this });
                                    //MessageBox.Show($"result={newContent}");
                                    AutomationCodeReturnData = newContent.ToString();
                                }
                                catch (Exception ex)
                                {
                                    _ = MessageBox.Show(ex.ToString());
                                }
                            });
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
