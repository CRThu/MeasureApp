using ICSharpCode.AvalonEdit.Document;
using MeasureApp.Model;
using MeasureApp.Model.Logger;
using MeasureApp.Model.SerialPortScript;
using Microsoft.Win32;
using Newtonsoft.Json;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MeasureApp.Model.Common;
using MeasureApp.ViewModel.Common;

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

        // 预设命令
        private ObservableCollection<SerialPortPresetCommand> serialPortPresetCommands = new();
        public ObservableCollection<SerialPortPresetCommand> SerialPortPresetCommands
        {
            get => serialPortPresetCommands;
            set
            {
                serialPortPresetCommands = value;
                RaisePropertyChanged(() => SerialPortPresetCommands);
            }
        }

        // 预设命令选中项绑定
        private SerialPortPresetCommand serialportPresetCommandSelectedItem;
        public SerialPortPresetCommand SerialportPresetCommandSelectedItem
        {
            get => serialportPresetCommandSelectedItem;
            set
            {
                serialportPresetCommandSelectedItem = value;
                RaisePropertyChanged(() => SerialportPresetCommandSelectedItem);
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
        private SerialPortCommLogger serialPortLogger = new();
        public SerialPortCommLogger SerialPortLogger
        {
            get => serialPortLogger;
            set
            {
                serialPortLogger = value;
                RaisePropertyChanged(() => SerialPortLogger);
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

        // 加载预设指令函数
        public void SerialPortLoadPresetCommandsFromJson(string jsonPath)
        {
            string json = File.ReadAllText(jsonPath);
            //var options = new JsonSerializerOptions
            //{
            //    IncludeFields = true
            //};
            //var ds = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<SerialPortPresetCommand>>(json, options);
            var ds = JsonConvert.DeserializeObject<ObservableCollection<SerialPortPresetCommand>>(json);
            SerialPortPresetCommands = ds;
        }

        // 串口发送回调
        private void SerialPortPreWriteString(string portName, string data)
        {
            if (portName == SerialportCommandPortNameSelectedValue)
            {
                SerialPortLogger.Add(data, "WPF", false);
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

                    SerialPortLogger.AddRange(msgCollection, "COM", Properties.Settings.Default.IsSerialPortLogKeywordHighlight);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // 完整脚本运行
        public void SerialPortCommandScriptRunAll()
        {
            System.Timers.Timer timer = new()
            {
                Interval = SerialportCommandScriptRunDelayMilliSecound,
            };
            timer.Elapsed += (sender, args) =>
            {
                timer.Stop();
                //Debug.WriteLine("TICK");
                var isContinue = SerialPortCommandScriptRunAllByTick();
                if (isContinue)
                    timer.Start();
            };
            timer.Start();
        }

        // 返回值 false:停止运行,true:继续运行
        public bool SerialPortCommandScriptRunAllByTick()
        {
            try
            {
                bool isContinue = true;
            start:
                string line = SerialPortCommandScriptGetCurrentLine();
                var status = SerialPortCommandScriptRunLine(line, isStopTagEnabled: SerialportCommandScriptIsRunToStop);
                // 运行到STOP则退出
                if (status == SerialPortScriptRunStatus.Stopped)
                {
                    isContinue = false;
                }
                // 运行到程序结尾则退出
                if (!SerialPortCommandScriptToNextLine())
                {
                    isContinue = false;
                }
                // 运行到空行则跳转执行下一行
                if (isContinue && status == SerialPortScriptRunStatus.BlankLine)
                {
                    goto start;
                }
                return isContinue;
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
                return false;
            }
        }

        // 脚本单行运行
        public SerialPortScriptRunStatus SerialPortCommandScriptRunLine(string line, bool isStopTagEnabled = true, bool isScriptTagEnabled = true)
        {
            string code = line.Split('#', 2).First().Trim();

            // 检测是否为空行
            if (string.IsNullOrWhiteSpace(code))
            {
                return SerialPortScriptRunStatus.BlankLine;
            }

            // 特殊命令解析
            if (XmlTag.IsMatchXmlTag(code))
            {
                Dictionary<string, string> TagAttrs = XmlTag.GetXmlTagAttrs(code);
                switch (TagAttrs["Tag"].ToUpper())
                {
                    case "STOP":
                        // <stop/>
                        if (isStopTagEnabled)
                        {
                            return SerialPortScriptRunStatus.Stopped;
                        }
                        break;
                    case "SCRIPT":
                        // <script code="..."/>
                        if (isScriptTagEnabled)
                        {
                            MessageBox.Show(TagAttrs.ContainsKey("code") ? TagAttrs["code"] : "No Codes.");
                        }
                        break;
                    case "DELAY":
                        // <delay time="..."/>
                        // default: time="1000"
                        Thread.Sleep(Convert.ToInt32(TagAttrs.ContainsKey("time") ? TagAttrs["time"] : "1000"));
                        break;
                    case "MSGBOX":
                        // <msgbox msg="..."/>
                        // default: msg="No Message."
                        MessageBox.Show(TagAttrs.ContainsKey("msg") ? TagAttrs["msg"] : "No Message.");
                        break;
                    case "WAIT":
                        // <wait keyword="..." timeout="..." stop="..."/>
                        // default: keyword="[COMMAND]" timeout="1000" stop="true"
                        //MessageBox.Show(SerialPortLogger.IsLastLogContains("COM", "[COMMAND]") ? "[COMMAND]" : "Nothing");
                        bool result = Utility.TimeoutCheck(Convert.ToInt32(TagAttrs.ContainsKey("timeout") ? TagAttrs["timeout"] : "1000"), () =>
                        {
                            try
                            {
                                while (!SerialPortLogger.IsLastLogContains("COM", TagAttrs.ContainsKey("keyword") ? TagAttrs["keyword"] : "[COMMAND]"))
                                    Thread.Sleep(20);
                                return true;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                                return false;
                            }
                        });
                        if (!result)
                        {
                            MessageBox.Show($"{code}: Timeout.");
                            if (Convert.ToBoolean(TagAttrs.ContainsKey("stop") ? TagAttrs["stop"] : "true"))
                                return SerialPortScriptRunStatus.Stopped;
                        }
                        break;
                    default:
                        throw new FormatException($"Unknown Command: {code}");
                }
            }
            else
            {
                // Debug.WriteLine($"[{SerialportCommandPortNameSelectedValue}]:{code}");
                SerialPortsInstance.WriteString(SerialportCommandPortNameSelectedValue, code);
            }

            return SerialPortScriptRunStatus.Executed;
        }

        // 获取本行脚本
        public string SerialPortCommandScriptGetCurrentLine()
        {
            return SerialPortCommandScriptGetLine(SerialportCommandScriptCurruntLineCursor);
        }

        // 获取行号脚本
        public string SerialPortCommandScriptGetLine(int lineCursor)
        {
            // To UI Thread
            return Application.Current.Dispatcher.Invoke(() =>
            {
                DocumentLine dl = SerialportCommandScriptEditorDocument.Lines[lineCursor - 1];
                string command = SerialportCommandScriptEditorDocument.Text.Substring(dl.Offset, dl.Length);
                //Debug.WriteLine($"{DateTime.Now}|{dl.Offset}+{dl.Length}:{command}");
                return command;
            });
        }

        // 跳转到下一行
        public bool SerialPortCommandScriptToNextLine()
        {
            // To UI Thread
            return Application.Current.Dispatcher.Invoke(() =>
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
            });
        }

        // 发送指令事件
        public void SerialPortCommandSend(object param)
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
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // 加载预设指令事件
        public void SerialPortCommandPresetLoad()
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
                    Properties.Settings.Default.DefaultPresetCommandsJsonPath = openFileDialog.FileName;
                    SerialPortLoadPresetCommandsFromJson(openFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // 串口预设指令发送
        public void SerialPortCommandPresetSend()
        {
            try
            {
                string com = SerialportCommandPortNameSelectedValue;
                string sendText = SerialportPresetCommandSelectedItem.Command;

                SerialPortsInstance.WriteString(com, sendText);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // 串口预设指令添加至Code
        public void SerialPortCommandPresetAddCode()
        {
            try
            {
                SerialportCommandScriptEditorDocument.Text += "\n" + SerialportPresetCommandSelectedItem.Command;
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // 串口监听打开/关闭事件
        public void SerialPortCommandListen()
        {
            try
            {
                if (!SerialPortCommandIsListeningDataReceived)
                {
                    SerialPortsInstance.AddDataReceivedEvent(SerialportCommandPortNameSelectedValue, SerialPortDataReceivedCallBack);
                    SerialPortsInstance.PreWriteString += SerialPortPreWriteString;
                }
                else
                {
                    SerialPortsInstance.RemoveDataReceivedEvent(SerialportCommandPortNameSelectedValue, SerialPortDataReceivedCallBack);
                    SerialPortsInstance.PreWriteString -= SerialPortPreWriteString;
                }
                SerialPortCommandIsListeningDataReceived = !SerialPortCommandIsListeningDataReceived;
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // 串口记录清空
        public void SerialPortCommandCleanLog()
        {
            try
            {
                SerialPortLogger.Clear();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // 串口记录导出
        public void SerialPortCommandSaveLog()
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
                    SerialPortLogger.Save(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // 串口命令模块加载脚本
        public void SerialPortCommandScriptLoadFile()
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
        }

        // 串口命令模块保存脚本
        public void SerialPortCommandScriptSaveFile()
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
        }

        // 串口命令模块运行脚本
        public void SerialPortCommandScriptRun(object param)
        {
            try
            {
                if (param is string)
                {
                    switch (param as string)
                    {
                        case "Run":
                            SerialPortCommandScriptRunAll();
                            break;
                        case "RunOnce":
                            string line = SerialPortCommandScriptGetCurrentLine();
                            SerialPortCommandScriptRunLine(line, isStopTagEnabled: false);
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
        }

        // CommandBase
        private CommandBase serialPortCommandSendEvent;
        public CommandBase SerialPortCommandSendEvent
        {
            get
            {
                if (serialPortCommandSendEvent == null)
                {
                    serialPortCommandSendEvent = new CommandBase(new Action<object>(param => SerialPortCommandSend(param)));
                }
                return serialPortCommandSendEvent;
            }
        }

        private CommandBase serialPortCommandPresetLoadEvent;
        public CommandBase SerialPortCommandPresetLoadEvent
        {
            get
            {
                if (serialPortCommandPresetLoadEvent == null)
                {
                    serialPortCommandPresetLoadEvent = new CommandBase(new Action<object>(param => SerialPortCommandPresetLoad()));
                }
                return serialPortCommandPresetLoadEvent;
            }
        }

        private CommandBase serialPortCommandPresetSendEvent;
        public CommandBase SerialPortCommandPresetSendEvent
        {
            get
            {
                if (serialPortCommandPresetSendEvent == null)
                {
                    serialPortCommandPresetSendEvent = new CommandBase(new Action<object>(param => SerialPortCommandPresetSend()));
                }
                return serialPortCommandPresetSendEvent;
            }
        }

        private CommandBase serialportCommandPresetAddCodeEvent;
        public CommandBase SerialportCommandPresetAddCodeEvent
        {
            get
            {
                if (serialportCommandPresetAddCodeEvent == null)
                {
                    serialportCommandPresetAddCodeEvent = new CommandBase(new Action<object>(param => SerialPortCommandPresetAddCode()));
                }
                return serialportCommandPresetAddCodeEvent;
            }
        }

        private CommandBase serialPortCommandListenEvent;
        public CommandBase SerialPortCommandListenEvent
        {
            get
            {
                if (serialPortCommandListenEvent == null)
                {
                    serialPortCommandListenEvent = new CommandBase(new Action<object>(param => SerialPortCommandListen()));
                }
                return serialPortCommandListenEvent;
            }
        }

        private CommandBase serialPortCommandCleanLogEvent;
        public CommandBase SerialPortCommandCleanLogEvent
        {
            get
            {
                if (serialPortCommandCleanLogEvent == null)
                {
                    serialPortCommandCleanLogEvent = new CommandBase(new Action<object>(param => SerialPortCommandCleanLog()));
                }
                return serialPortCommandCleanLogEvent;
            }
        }

        private CommandBase serialPortCommandSaveLogEvent;
        public CommandBase SerialPortCommandSaveLogEvent
        {
            get
            {
                if (serialPortCommandSaveLogEvent == null)
                {
                    serialPortCommandSaveLogEvent = new CommandBase(new Action<object>(param => SerialPortCommandSaveLog()));
                }
                return serialPortCommandSaveLogEvent;
            }
        }

        private CommandBase serialPortCommandScriptLoadFileEvent;
        public CommandBase SerialPortCommandScriptLoadFileEvent
        {
            get
            {
                if (serialPortCommandScriptLoadFileEvent == null)
                {
                    serialPortCommandScriptLoadFileEvent = new CommandBase(new Action<object>(param => SerialPortCommandScriptLoadFile()));
                }
                return serialPortCommandScriptLoadFileEvent;
            }
        }

        private CommandBase serialPortCommandScriptSaveFileEvent;
        public CommandBase SerialPortCommandScriptSaveFileEvent
        {
            get
            {
                if (serialPortCommandScriptSaveFileEvent == null)
                {
                    serialPortCommandScriptSaveFileEvent = new CommandBase(new Action<object>(param => SerialPortCommandScriptSaveFile()));
                }
                return serialPortCommandScriptSaveFileEvent;
            }
        }

        private CommandBase serialPortCommandScriptRunEvent;
        public CommandBase SerialPortCommandScriptRunEvent
        {
            get
            {
                if (serialPortCommandScriptRunEvent == null)
                {
                    serialPortCommandScriptRunEvent = new CommandBase(new Action<object>(param => SerialPortCommandScriptRun(param)));
                }
                return serialPortCommandScriptRunEvent;
            }
        }
    }
}
