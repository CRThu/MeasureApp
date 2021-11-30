using ICSharpCode.AvalonEdit.Document;
using MeasureApp.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 串口选择
        private string serialportCommandPortNameSelectedValue;
        public string SerialportCommandPortNameSelectedValue
        {
            get => serialportCommandPortNameSelectedValue;
            set
            {
                serialportCommandPortNameSelectedValue = value;
                RaisePropertyChanged(() => SerialportCommandPortNameSelectedValue);
            }
        }

        // 打开监听标志位
        private bool serialPortCommandIsListeningDataReceived;
        public bool SerialPortCommandIsListeningDataReceived
        {
            get => serialPortCommandIsListeningDataReceived;
            set
            {
                serialPortCommandIsListeningDataReceived = value;
                RaisePropertyChanged(() => SerialPortCommandIsListeningDataReceived);
            }
        }


        // GUI绑定
        // TODO
        private ObservableCollection<SerialportCommandModel> serialportCommandModels = new()
        {
            new SerialportCommandModel("初始化", 0, "RESET"),
            new SerialportCommandModel("初始化", 0, "OPEN"),
            new SerialportCommandModel("写字节", 1, "DATW"),
            new SerialportCommandModel("读字节", 0, "DATR"),
            new SerialportCommandModel("写寄存器", 2, "REGW"),
            new SerialportCommandModel("读寄存器", 1, "REGR"),
            new SerialportCommandModel("写寄存器位", 3, "REGM",
                (p) =>
                {
                    int[] regBitsPos = p[1].Split(':').Select(s => Convert.ToInt32(s)).ToArray();
                    if (regBitsPos.Length != 2)
                        throw new ArgumentOutOfRangeException($"regBitsPos.Count() = {regBitsPos.Length}");
                    int regBitsMSB = regBitsPos.Max();
                    int regBitsLSB = regBitsPos.Min();
                    int regBitsLen = regBitsMSB - regBitsLSB + 1;
                    // Modify REG21[10:0]=0x180
                    // REGM;21;0;11;180;
                    return new dynamic[] { p[0], regBitsLSB, regBitsLen, p[2] };
                }),
            new SerialportCommandModel("读寄存器位", 2, "REGQ",
                (p) =>
                {
                    int[] regBitsPos = p[1].Split(':').Select(s => Convert.ToInt32(s)).ToArray();
                    if (regBitsPos.Length != 2)
                        throw new ArgumentOutOfRangeException($"regBitsPos.Count() = {regBitsPos.Length}");
                    int regBitsMSB = regBitsPos.Max();
                    int regBitsLSB = regBitsPos.Min();
                    int regBitsLen = regBitsMSB - regBitsLSB + 1;
                    // Query REG21[10:0]=0x180
                    // REGQ;21;0;11;
                    return new dynamic[] { p[0], regBitsLSB, regBitsLen };
                }),
        };

        public ObservableCollection<SerialportCommandModel> SerialportCommandModels
        {
            get => serialportCommandModels;
            set
            {
                serialportCommandModels = value;
                RaisePropertyChanged(() => SerialportCommandModels);
            }
        }

        // 串口命令脚本编辑器数据绑定
        private TextDocument serialportCommandScriptEditorDocument = new();
        public TextDocument SerialportCommandScriptEditorDocument
        {
            get => serialportCommandScriptEditorDocument;
            set
            {
                serialportCommandScriptEditorDocument = value;
                RaisePropertyChanged(() => SerialportCommandScriptEditorDocument);
            }
        }

        // 串口命令模块监听
        private object serialPortCommandLoglocker = new();
        private ObservableRangeCollection<SerialPortCommLog> serialportCommandLog = new();
        public ObservableRangeCollection<SerialPortCommLog> SerialportCommandLog
        {
            get => serialportCommandLog;
            set
            {
                serialportCommandLog = value;
                RaisePropertyChanged(() => SerialportCommandLog);
            }
        }

        // 串口命令模块脚本运行延时
        private int serialportCommandScriptRunDelayMilliSecound = 100;
        public int SerialportCommandScriptRunDelayMilliSecound
        {
            get => serialportCommandScriptRunDelayMilliSecound;
            set
            {
                serialportCommandScriptRunDelayMilliSecound = value;
                RaisePropertyChanged(() => SerialportCommandScriptRunDelayMilliSecound);
            }
        }

        // 运行至STOP CheckBox数据绑定
        private bool serialportCommandScriptIsRunToStop = true;
        public bool SerialportCommandScriptIsRunToStop
        {
            get => serialportCommandScriptIsRunToStop;
            set
            {
                serialportCommandScriptIsRunToStop = value;
                RaisePropertyChanged(() => SerialportCommandScriptIsRunToStop);
            }
        }

        // 运行到行数游标数据绑定
        private int serialportCommandScriptCurruntLineCursor = 1;
        public int SerialportCommandScriptCurruntLineCursor
        {
            get => serialportCommandScriptCurruntLineCursor;
            set
            {
                serialportCommandScriptCurruntLineCursor = value;
                RaisePropertyChanged(() => SerialportCommandScriptCurruntLineCursor);
            }
        }

        // 发送指令事件
        private CommandBase serialportCommandSendEvent;
        public CommandBase SerialportCommandSendEvent
        {
            get
            {
                if (serialportCommandSendEvent == null)
                {
                    serialportCommandSendEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            if (param is string)
                            {
                                int index = Convert.ToInt32(param);
                                string com = SerialportCommandPortNameSelectedValue;
                                List<dynamic> vs = new(SerialportCommandModels[index].SendParamsTexts);
                                vs.Insert(0, SerialportCommandModels[index].CommandText);
                                string sendText = $"{string.Join(";", vs)};";
                                SerialPortsInstance.WriteString(com, sendText);

                                lock (serialPortCommandLoglocker)
                                {
                                    SerialportCommandLog.Add(new SerialPortCommLog("WPF", sendText));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return serialportCommandSendEvent;
            }
        }

        // 串口监听打开/关闭事件
        private CommandBase serialPortCommandListenEvent;
        public CommandBase SerialPortCommandListenEvent
        {
            get
            {
                if (serialPortCommandListenEvent == null)
                {
                    serialPortCommandListenEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            if (!SerialPortCommandIsListeningDataReceived)
                            {
                                SerialPortsInstance.AddDataReceivedEvent(SerialportCommandPortNameSelectedValue, SerialPortDataReceivedCallBack);
                            }
                            else
                            {
                                SerialPortsInstance.RemoveDataReceivedEvent(SerialportCommandPortNameSelectedValue, SerialPortDataReceivedCallBack);
                            }
                            SerialPortCommandIsListeningDataReceived = !SerialPortCommandIsListeningDataReceived;
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return serialPortCommandListenEvent;
            }
        }

        // 串口记录清空
        private CommandBase serialPortCommandCleanLogEvent;
        public CommandBase SerialPortCommandCleanLogEvent
        {
            get
            {
                if (serialPortCommandCleanLogEvent == null)
                {
                    serialPortCommandCleanLogEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            SerialportCommandLog.Clear();
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return serialPortCommandCleanLogEvent;
            }
        }

        // 串口记录导出
        private CommandBase serialPortCommandSaveLogEvent;
        public CommandBase SerialPortCommandSaveLogEvent
        {
            get
            {
                if (serialPortCommandSaveLogEvent == null)
                {
                    serialPortCommandSaveLogEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            SaveFileDialog saveFileDialog = new()
                            {
                                Title = "存储数据",
                                FileName = "SerialPortLog",
                                DefaultExt = ".log",
                                Filter = "Log File|*.log",
                                InitialDirectory = Properties.Settings.Default.DefaultDirectory
                            };
                            if (saveFileDialog.ShowDialog() == true)
                            {
                                Properties.Settings.Default.DefaultDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                                File.WriteAllLines(saveFileDialog.FileName, SerialportCommandLog.Select(l => l.ToString()));
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return serialPortCommandSaveLogEvent;
            }
        }

        // 串口监听回调
        private void SerialPortDataReceivedCallBack(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int _bytesToRead = SerialPortsInstance.SerialPortsDict[SerialportCommandPortNameSelectedValue].BytesToRead;
                if (_bytesToRead > 0)
                {
                    string[] msgCollection = SerialPortsInstance.ReadExistingString(SerialportCommandPortNameSelectedValue).Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    lock (serialPortCommandLoglocker)
                    {
                        SerialportCommandLog.AddRange(msgCollection.Select(l => new SerialPortCommLog("COM", l)));
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // 串口命令模块加载脚本
        private CommandBase serialportCommandScriptLoadFileEvent;
        public CommandBase SerialportCommandScriptLoadFileEvent
        {
            get
            {
                if (serialportCommandScriptLoadFileEvent == null)
                {
                    serialportCommandScriptLoadFileEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            // Open File Dialog
                            OpenFileDialog openFileDialog = new()
                            {
                                Title = "Open Script File...",
                                Filter = "Text File|*.txt|Task Code|*.task",
                                InitialDirectory = Properties.Settings.Default.DefaultDirectory
                            };
                            if (openFileDialog.ShowDialog() == true)
                            {
                                Properties.Settings.Default.DefaultDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                                SerialportCommandScriptEditorDocument.Text = File.ReadAllText(openFileDialog.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return serialportCommandScriptLoadFileEvent;
            }
        }

        // 串口命令模块保存脚本
        private CommandBase serialportCommandScriptSaveFileEvent;
        public CommandBase SerialportCommandScriptSaveFileEvent
        {
            get
            {
                if (serialportCommandScriptSaveFileEvent == null)
                {
                    serialportCommandScriptSaveFileEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            // Save File Dialog
                            SaveFileDialog saveFileDialog = new()
                            {
                                Title = "Save Script File...",
                                Filter = "Text File|*.txt|Task Code|*.task",
                                InitialDirectory = Properties.Settings.Default.DefaultDirectory
                            };
                            if (saveFileDialog.ShowDialog() == true)
                            {
                                Properties.Settings.Default.DefaultDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                                File.WriteAllText(saveFileDialog.FileName, SerialportCommandScriptEditorDocument.Text);
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return serialportCommandScriptSaveFileEvent;
            }
        }


        // 串口命令模块运行脚本
        private CommandBase serialportCommandScriptRunEvent;
        public CommandBase SerialportCommandScriptRunEvent
        {
            get
            {
                if (serialportCommandScriptRunEvent == null)
                {
                    serialportCommandScriptRunEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            if (param is string)
                            {
                                switch (param as string)
                                {
                                    case "Run":
                                        DispatcherTimer timer = new()
                                        {
                                            Interval = TimeSpan.FromMilliseconds(SerialportCommandScriptRunDelayMilliSecound),

                                        };
                                        timer.Tick += (sender, args) =>
                                        {
                                            try
                                            {
                                                // 运行到STOP则退出
                                                if (!SerialPortCommandRunScriptCurrentLine(isStopTagEnabled: SerialportCommandScriptIsRunToStop))
                                                {
                                                    timer.Stop();
                                                }

                                                // 运行到程序结尾则退出
                                                if (!SerialPortCommandScriptToNextLine())
                                                {
                                                    timer.Stop();
                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                _ = MessageBox.Show(ex.ToString());
                                                timer.Stop();
                                            }
                                        };
                                        timer.Start();
                                        break;
                                    case "RunOnce":
                                        SerialPortCommandRunScriptCurrentLine(isStopTagEnabled: false);
                                        SerialPortCommandScriptToNextLine();
                                        break;
                                    case "ClearCursor":
                                        SerialportCommandScriptCurruntLineCursor = 1;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return serialportCommandScriptRunEvent;
            }
        }

        public bool SerialPortCommandRunScriptCurrentLine(bool isStopTagEnabled = true, bool isScriptTagEnabled = true)
        {
            string line = SerialPortCommandScriptGetCurrentLine();
            string code = line.Split('#', 2).First().Trim();

            // 特殊命令解析
            if (IsMatchHtmlTag(code))
            {
                if (IsMatchHtmlTag(code, "stop"))
                {
                    if (isStopTagEnabled)
                    {
                        // TODO
                        //MessageBox.Show(MatchHtmlTag(code, "stop"));
                        return false;
                    }
                }
                else if (IsMatchHtmlTag(code, "script"))
                {
                    if (isScriptTagEnabled)
                    {
                        // TODO
                        //MessageBox.Show(MatchHtmlTag(code, "script"));
                    }
                }
                else if (IsMatchHtmlTag(code, "delay"))
                {
                    if (isScriptTagEnabled)
                    {
                        // 阻塞ui
                        Thread.Sleep(Convert.ToInt32(MatchHtmlTag(code, "delay")));
                    }
                }
                else
                {
                    throw new FormatException($"Unknown Command: {code}");
                }
            }
            else
            {
                // Debug.WriteLine($"[{SerialportCommandPortNameSelectedValue}]:{code}");
                SerialPortsInstance.WriteString(SerialportCommandPortNameSelectedValue, code);
            }

            return true;
        }

        public static bool IsMatchHtmlTag(string codeString)
        {
            string regexStr = @"<[^>]+>";
            return Regex.IsMatch(codeString, regexStr, RegexOptions.IgnoreCase);
        }

        public static bool IsMatchHtmlTag(string codeString, string TagName)
        {
            string regexStr = @$"<{TagName}[^>]*?>[\s\S]*?</{TagName}>";
            return Regex.IsMatch(codeString, regexStr, RegexOptions.IgnoreCase);
        }

        public static string MatchHtmlTag(string codeString, string TagName)
        {
            string regexStr = @$"<{TagName}[^>]*?>(?<Attribute>[\s\S]*)</{TagName}>";
            //string regexStr = @$"<{TagName}[^>]*?>(?<Attribute>[^<]*)</{TagName}>";
            Match match = Regex.Match(codeString, regexStr, RegexOptions.IgnoreCase);
            // Debug.WriteLine($"{match.Groups["Attribute"].Value}");
            return match.Groups["Attribute"].Value;
        }

        public string SerialPortCommandScriptGetCurrentLine()
        {
            return SerialPortCommandScriptGetLine(SerialportCommandScriptCurruntLineCursor);
        }

        public string SerialPortCommandScriptGetLine(int lineCursor)
        {
            DocumentLine dl = SerialportCommandScriptEditorDocument.Lines[lineCursor - 1];
            string command = SerialportCommandScriptEditorDocument.Text.Substring(dl.Offset, dl.Length);
            // Debug.WriteLine($"{dl.Offset}+{dl.Length}({dl.IsDeleted}):{command}");
            return command;
        }

        public bool SerialPortCommandScriptToNextLine()
        {
            if (SerialportCommandScriptCurruntLineCursor < SerialportCommandScriptEditorDocument.LineCount)
            {
                SerialportCommandScriptCurruntLineCursor++;
                return true;
            }
            else
            {
                SerialportCommandScriptCurruntLineCursor = 1;
                return false;
            }
        }

    }
}
