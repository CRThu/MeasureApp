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
using MeasureApp.Model.Devices;
using CodingSeb.ExpressionEvaluator;
using MeasureApp.Model.DataStorage;

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

        /// <summary>
        /// 脚本寄存器字典
        /// </summary>
        private ObservableDictionary<string, decimal> serialportCommandScriptVarDict = new();
        public ObservableDictionary<string, decimal> SerialportCommandScriptVarDict
        {
            get => serialportCommandScriptVarDict;
            set
            {
                serialportCommandScriptVarDict = value;
                RaisePropertyChanged(() => SerialportCommandScriptVarDict);
            }
        }

        // 串口脚本模块是否运行标志
        private bool serialportCommandScriptIsRun = false;
        public bool SerialportCommandScriptIsRun
        {
            get => serialportCommandScriptIsRun;
            set
            {
                serialportCommandScriptIsRun = value;
                RaisePropertyChanged(() => SerialportCommandScriptIsRun);
            }
        }

        // 串口通信采集指令默认键名称
        private string serialportMeasureDefaultKeyName = Key3458AString;
        public string SerialportMeasureDefaultKeyName
        {
            get => serialportMeasureDefaultKeyName;
            set
            {
                serialportMeasureDefaultKeyName = value;
                RaisePropertyChanged(() => SerialportMeasureDefaultKeyName);
            }
        }

        /// <summary>
        /// 脚本For信息栈字典
        /// 0|
        /// 1|for(A)
        /// 2|    for(B)
        /// 3|        for(C)
        /// 4|        ...
        /// 5|        forend(C)
        /// 6|    forend(B)
        /// 7|forend(A)
        /// 8|
        /// For信息栈内存放为
        /// |0|1|2|3|4|5|6|7|8|
        /// |-|A|A|A|A|A|A|A|-|
        /// |-|-|B|B|B|B|B|-|-|
        /// |-|-|-|C|C|C|-|-|-|
        /// </summary>
        private ObservableCollection<CommandScriptForStatementInfo> serialportCommandScriptForStack = new();
        public ObservableCollection<CommandScriptForStatementInfo> SerialportCommandScriptForStack
        {
            get => serialportCommandScriptForStack;
            set
            {
                serialportCommandScriptForStack = value;
                RaisePropertyChanged(() => SerialportCommandScriptForStack);
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
                // 若被停止则不再启动Timer
                if (SerialportCommandScriptIsRun)
                {
                    //Debug.WriteLine("TICK");
                    var isContinue = SerialPortCommandScriptRunAllByTick();
                    if (isContinue)
                    {
                        timer.Start();
                    }
                    else
                    {
                        // 停止
                        SerialportCommandScriptIsRun = false;
                    }
                }
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
                        // <wait keyword="..." timeout="..." stop="..." storekey="..."/>
                        // default: keyword="[COMMAND]" timeout="1000" stop="true"
                        // expr = true/false, default: false
                        /*
PA5.FREQ;
<wait keyword="FREQ" timeout="1500" storekey="f"/>
PA5.FREQ;
<setvar key="i" val="123"/>
<wait keyword="FREQ" timeout="1500" expr="true" storekey="'f@'+i"/>
                         */

                        string waitKeyword0 = TagAttrs.ContainsKey("keyword") ? TagAttrs["keyword"] : "[COMMAND]";
                        string waitTimeout0 = TagAttrs.ContainsKey("timeout") ? TagAttrs["timeout"] : "1000";
                        string waitStop0 = TagAttrs.ContainsKey("stop") ? TagAttrs["stop"] : "true";
                        string waitStoreKey0 = TagAttrs.ContainsKey("storekey") ? TagAttrs["storekey"] : null;
                        string waitExpr0 = TagAttrs.ContainsKey("expr") ? TagAttrs["expr"] : "false";

                        if (Convert.ToBoolean(waitExpr0))
                        {
                            // 支持表达式运算
                            ExpressionEvaluator evaluator4 = new();
                            evaluator4.Variables = SerialportCommandScriptVarDict.ToDictionary(pair => pair.Key, pair => (object)(double)pair.Value);
                            waitStoreKey0 = evaluator4.Evaluate(waitStoreKey0.Replace('\'', '\"')).ToString();
                        }

                        bool result = Utility.TimeoutCheck(Convert.ToInt32(waitTimeout0), () =>
                        {
                            try
                            {
                                while (!SerialPortLogger.IsLastLogContains("COM", waitKeyword0))
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
                            if (Convert.ToBoolean(waitStop0))
                                return SerialPortScriptRunStatus.Stopped;
                        }
                        else
                        {
                            // storekey
                            if (waitStoreKey0 != null)
                            {
                                SerialPortCommLogElement lastLogFromHost = SerialPortLogger.LogCollection.LastOrDefault(log => log.Host == "COM");
                                if (lastLogFromHost is not null && ((string)lastLogFromHost.Message.ToString()).Contains(waitKeyword0, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    string[] storeKeyStrs = ((string)lastLogFromHost.Message.ToString()).Split(':');
                                    decimal storeKeyData = Convert.ToDecimal(storeKeyStrs[1].Trim());
                                    DataStorageInstance.AddValue(waitStoreKey0, storeKeyData);
                                }
                            }
                        }
                        break;
                    case "SERIALPORT":
                        // 临时
                        // <serialport port="..." cmd="..."/>
                        /*
<serialport port="COM10" cmd="'Measure'"/>
                        
<setvar key="i" val="123"/>
<serialport port="COM10" cmd="'Measure_'+i"/>
                        */

                        string serialportPortName = TagAttrs["port"];
                        string serialportCmd = TagAttrs["cmd"];

                        // 支持表达式运算
                        ExpressionEvaluator evaluator1 = new();
                        evaluator1.Variables = SerialportCommandScriptVarDict.ToDictionary(pair => pair.Key, pair => (object)(double)pair.Value);
                        string r1 = evaluator1.Evaluate(serialportCmd.Replace('\'', '\"')).ToString();


                        SerialPortsInstance.WriteString(serialportPortName, r1 + "\r\n");
                        break;

                    case "GPIB":
                        // 临时
                        // <gpibmeasure addr="..." mode="..." expr="..."/>
                        // mode = DCI DCV NPLC
                        // expr = true/false, default: false
                        /*
<gpib addr="GPIB0::22::INSTR" mode="NPLC 10"/>
<gpib addr="GPIB0::22::INSTR" mode="DCI"/>
<gpib addr="GPIB0::18::INSTR" mode="SENS:CURR:NPLC 10"/>
<gpib addr="GPIB0::18::INSTR" mode="DISP:CURR:DIG 6"/>
<gpib addr="GPIB0::18::INSTR" mode="SOUR:VOLT 1.65"/>
<gpib addr="GPIB0::18::INSTR" mode="OUTP ON"/>
                        
<setvar key="volt" val="1.65"/>
<gpib addr="GPIB0::18::INSTR" expr="true" mode="'SOUR:VOLT '+volt"/>
                        */
                        string gpibAddr0 = TagAttrs.ContainsKey("addr") ? TagAttrs["addr"] : GpibDevicesName.First();
                        string gpibCmd0 = TagAttrs.ContainsKey("mode") ? TagAttrs["mode"] : string.Empty;
                        string CmdExpr0 = TagAttrs.ContainsKey("expr") ? TagAttrs["expr"] : "false";

                        if (Convert.ToBoolean(CmdExpr0))
                        {
                            // 支持表达式运算
                            ExpressionEvaluator evaluator3 = new();
                            evaluator3.Variables = SerialportCommandScriptVarDict.ToDictionary(pair => pair.Key, pair => (object)(double)pair.Value);
                            gpibCmd0 = evaluator3.Evaluate(gpibCmd0.Replace('\'', '\"')).ToString();
                        }

                        GPIB3458AMeasure gpibDevice = new();
                        gpibDevice.Open(gpibAddr0);
                        gpibDevice.Timeout = Properties.Settings.Default.GPIBTimeout;
                        if (gpibDevice.IsOpen)
                        {
                            try
                            {
                                if (gpibCmd0 != string.Empty)
                                {
                                    gpibDevice.WriteCommand(gpibCmd0);
                                }
                                gpibDevice.Dispose();
                            }
                            catch (Exception ex)
                            {
                                _ = MessageBox.Show(ex.ToString());
                                gpibDevice.Dispose();
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("GPIB is not open.");
                        }
                        break;
                    case "MEASUREKEY":
                        // 临时
                        // <measurekey key="..."/>
                        // default: Key = 3458A Data Storage
                        /*
<measurekey key="'Measure'"/>

<setvar key="i" val="123"/>
<measurekey key="'Measure_'+i"/>
                        */
                        SerialportMeasureDefaultKeyName = TagAttrs.ContainsKey("key") ? TagAttrs["key"] : Key3458AString;

                        // 支持表达式运算
                        ExpressionEvaluator evaluator2 = new();
                        evaluator2.Variables = SerialportCommandScriptVarDict.ToDictionary(pair => pair.Key, pair => (object)(double)pair.Value);
                        string r2 = evaluator2.Evaluate(SerialportMeasureDefaultKeyName.Replace('\'', '\"')).ToString();

                        SerialportMeasureDefaultKeyName = r2;
                        break;
                    case "GPIBMEASURE":
                        // 临时
                        // <gpibmeasure addr="..." key="..." mode="..."/>
                        // default: Key = 3458A Data Storage
                        // mode = DCI DCV <null>
                        /*
<gpibmeasure addr="GPIB0::22::INSTR" key="Measure" mode="DCV"/>
<gpibmeasure addr="GPIB0::22::INSTR" key="Measure" mode="DCI"/>
<gpibmeasure addr="GPIB0::19::INSTR" key="Measure" mode=":MEAS:VOLT:DC?"/>
<gpibmeasure addr="GPIB0::19::INSTR" key="Measure" mode=":MEAS:CURR:DC?"/>
                        */
                        string measureGpibAddr0 = TagAttrs.ContainsKey("addr") ? TagAttrs["addr"] : GpibDevicesName.First();
                        string measureKeyName0 = TagAttrs.ContainsKey("key") ? TagAttrs["key"] : SerialportMeasureDefaultKeyName;
                        string measureCmd0 = TagAttrs.ContainsKey("mode") ? TagAttrs["mode"] : string.Empty;
                        M3458AManualMeasureText = "Measuring...";
                        GPIB3458AMeasure measureDevice = new();
                        measureDevice.Open(measureGpibAddr0);
                        measureDevice.Timeout = Properties.Settings.Default.GPIBTimeout;
                        if (measureDevice.IsOpen)
                        {
                            try
                            {
                                decimal measureData = measureCmd0 == string.Empty ? measureDevice.ReadDecimal() : measureDevice.QueryDecimal(measureCmd0);
                                M3458AManualMeasureText = measureData.ToString();
                                DataStorageInstance.AddValue(measureKeyName0, measureData);
                                measureDevice.Dispose();
                            }
                            catch (Exception ex)
                            {
                                _ = MessageBox.Show(ex.ToString());
                                measureDevice.Dispose();
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("GPIB is not open.");
                        }
                        break;
                    case "MEASURE":
                        // <measure key="..." mode="..."/>
                        // default: Key = 3458A Data Storage
                        // mode = DCI DCV <null>
                        /*
<measure key="Measure" mode="DCV"/>
<measure key="Measure" mode="DCI"/>
<measure key="Measure"/>
                        */
                        string measureKeyName = TagAttrs.ContainsKey("key") ? TagAttrs["key"] : SerialportMeasureDefaultKeyName;
                        string measureCmd = TagAttrs.ContainsKey("mode") ? TagAttrs["mode"] : string.Empty;
                        M3458AManualMeasureText = "Measuring...";
                        if (Measure3458AInstance.IsOpen)
                        {
                            try
                            {
                                // 3458A 连续发送DCV时千分之一概率出现超时错误,需要重发
                                decimal measureData;
                                int retryTimes = 3;
                                while (true)
                                {
                                    try
                                    {
                                        measureData = measureCmd == string.Empty ? Measure3458AInstance.ReadDecimal() : Measure3458AInstance.QueryDecimal(measureCmd);
                                        break;
                                    }
                                    catch
                                    {
                                        retryTimes--;
                                        Debug.WriteLine($"{DateTime.Now:HH:mm:ss}: Visa has throwed an exception, retryTimes: {retryTimes}");
                                        if (retryTimes <= 0)
                                            throw;
                                    }
                                }
                                M3458AManualMeasureText = measureData.ToString();
                                DataStorageInstance.AddValue(measureKeyName, measureData);
                            }
                            catch (Exception ex)
                            {
                                _ = MessageBox.Show(ex.ToString());
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("GPIB(3458A) is not open.");
                        }
                        break;
                    case "DELMEASURE":
                        // <delmeasure key="..."/>
                        // default:all
                        string delmeaKey0 = TagAttrs.ContainsKey("key") ? TagAttrs["key"] : null;

                        if (delmeaKey0 != null)
                        {
                            if (DataStorageInstance.ContainsKey(delmeaKey0))
                                DataStorageInstance.RemoveKey(delmeaKey0);
                        }
                        else
                        {
                            foreach(var key in DataStorageInstance.Keys)
                                DataStorageInstance.RemoveKey(key);
                        }
                        break;
                    case "SETVAR":
                        // <setvar key="..." val="..."/>
                        // <setvar key="i" val="123"/>
                        if (TagAttrs.ContainsKey("key") && TagAttrs.ContainsKey("val"))
                        {
                            SerialportCommandScriptVarDict[TagAttrs["key"]] = Convert.ToDecimal(TagAttrs["val"]);
                        }
                        else
                        {
                            throw new InvalidOperationException("SETVAR do not contain key or value attribute.");
                        }
                        break;
                    case "DELVAR":
                        // <delvar key="..."/>
                        if (TagAttrs.ContainsKey("key"))
                        {
                            if (SerialportCommandScriptVarDict.ContainsKey(TagAttrs["key"]))
                                SerialportCommandScriptVarDict.Remove(TagAttrs["key"]);
                        }
                        else
                        {
                            throw new InvalidOperationException("DELVAR do not contain key attribute.");
                        }
                        break;
                    case "GOTO":
                        // <goto line="..."/>
                        if (TagAttrs.ContainsKey("line"))
                        {
                            int gotoLine = Convert.ToInt32(TagAttrs["line"]);
                            SerialPortCommandScriptGotoLinePointer(gotoLine);
                        }
                        else
                        {
                            throw new InvalidOperationException("GOTO do not contain line attribute.");
                        }
                        break;
                    case "FOR":
                        // <for var="..." begin="..." end="..." step="..."/>
                        // var add from begin to end, begin < end
                        // default: var = FOR_ITERATOR
                        // default: Step = 1
                        // type
                        // var:string
                        // begin:int32
                        // end:int32
                        // step:int32

                        /*
A;
<for var="i" begin="1" end="2" step="1"/>
<for var="j" begin="1" end="3" step="1"/>
REGW;{i:X};{j:D};
<forend/>
<forend/>
B;
<for var="i" begin="1.8" end="1.4" step="-0.1"/>
<for var="j" begin="1" end="-1" step="-1"/>
REGW;{i:F3};{j:F3};
<forend/>
<forend/>
                         */
                        if (TagAttrs.ContainsKey("begin") && TagAttrs.ContainsKey("end"))
                        {
                            string forVarName = TagAttrs.ContainsKey("var") ? TagAttrs["var"] : "FOR_ITERATOR";
                            decimal begin = Convert.ToDecimal(TagAttrs["begin"]);
                            decimal end = Convert.ToDecimal(TagAttrs["end"]);
                            decimal step = Convert.ToDecimal(TagAttrs.ContainsKey("step") ? TagAttrs["step"] : "1");

                            // ForStackDict: FOR[VAR]
                            int currentLine = SerialPortCommandScriptGetCurrentLinePointer();
                            // 第一次进入此for循环
                            if (SerialportCommandScriptForStack.Where(l => l.ForPointer == currentLine).Count() == 0)
                            {
                                SerialportCommandScriptForStack.Add(new CommandScriptForStatementInfo()
                                {
                                    ForPointer = currentLine,
                                    EndForPointer = -1,
                                    Var = forVarName,
                                    Begin = begin,
                                    End = end,
                                    Step = step,
                                });
                                SerialportCommandScriptVarDict[forVarName] = begin;
                            }
                            // 循环状态
                            var getForInfoFromStack0 = SerialportCommandScriptForStack[^1];
                            if ((SerialportCommandScriptVarDict[getForInfoFromStack0.Var] > getForInfoFromStack0.End && getForInfoFromStack0.Step > 0)
                                || (SerialportCommandScriptVarDict[getForInfoFromStack0.Var] < getForInfoFromStack0.End && getForInfoFromStack0.Step < 0))
                            {
                                // 循环判断语句为false
                                SerialPortCommandScriptGotoLinePointer(getForInfoFromStack0.EndForPointer + 1);
                                SerialportCommandScriptForStack.RemoveAt(SerialportCommandScriptForStack.Count - 1);
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("FOR do not contain key or value attribute.");
                        }
                        break;
                    case "FOREND":
                        // <forend/>
                        // 自增
                        var getForInfoFromStack = SerialportCommandScriptForStack[^1];
                        // 若第一次运行至forend则保存endfor指针
                        if (getForInfoFromStack.EndForPointer == -1)
                            getForInfoFromStack.EndForPointer = SerialPortCommandScriptGetCurrentLinePointer();
                        SerialportCommandScriptVarDict[getForInfoFromStack.Var] += getForInfoFromStack.Step;
                        SerialPortCommandScriptGotoLinePointer(getForInfoFromStack.ForPointer);
                        break;
                    case "TRIM":
                        // <trim key="..." target="..." retvar="..." rettrimdata="..."/>
                        // trim指令寻找datastorage的key数组中最接近target的目标值并返回index存入retvar名称的寄存器
                        /*
<trim key="keyX" target="2.5" retvar="i" rettrimdata="x"/>
REGW;01;{i:D};
                         */
                        string trimDataKey = TagAttrs["key"];
                        double trimTargetValue = Convert.ToDouble(TagAttrs["target"]);
                        string trimReturnVar = TagAttrs["retvar"];
                        IEnumerable<double> data = DataStorageInstance[trimDataKey];
                        double closestValue = data.MinBy(x => Math.Abs(x - trimTargetValue));
                        if (TagAttrs.ContainsKey("rettrimdata"))
                            SerialportCommandScriptVarDict[TagAttrs["rettrimdata"]] = (decimal)closestValue;
                        SerialportCommandScriptVarDict[trimReturnVar] = Array.IndexOf(data.ToArray(), closestValue);
                        break;
                    default:
                        throw new FormatException($"Unknown Command: {code}");
                }
            }
            else
            {
                try
                {
                    /*
<setvar key="i" val="255"/>
<setvar key="j" val="123"/>
REGW;{i:X};{j:D};

<setvar key="i" val="255"/>
<setvar key="j" val="123"/>
REGW;{i};{j};

<setvar key="i" val="1.1"/>
<setvar key="j" val="-2.2"/>
REGW;{i+j+3:D};{Round(j+8):D};{Max(i,j,0.5):F3};
                     */

                    // 语法解析 REGW;{i:X};{j:D};{i+2:D};
                    var leftSymbolIndexes = code.Select((item, index) => new { item, index }).Where(t => t.item == '{').Select(t => t.index).ToArray();
                    var rightSymbolIndexes = code.Select((item, index) => new { item, index }).Where(t => t.item == '}').Select(t => t.index).ToArray();
                    var symboIndexes = leftSymbolIndexes.Zip(rightSymbolIndexes).ToArray();
                    List<string> splitStrs = new();
                    List<int> splitStrsReplaceIndex = new();
                    int splitStrsCursor = 0;
                    // 将带转义代码分割并填入splitStrs,转义代码索引填入splitStrsReplaceIndex
                    // REGW;{i:X};{j:D};{i+2:D};
                    for (int i = 0; i < symboIndexes.Length; i++)
                    {
                        splitStrs.Add(code[splitStrsCursor..symboIndexes[i].First]);
                        splitStrs.Add(code[symboIndexes[i].First..(symboIndexes[i].Second + 1)]);
                        splitStrsCursor = symboIndexes[i].Second + 1;
                        splitStrsReplaceIndex.Add(splitStrs.Count - 1);
                    }
                    if (splitStrsCursor != code.Length)
                        splitStrs.Add(code[splitStrsCursor..code.Length]);

                    // {i:X} {j:D} {i+2:D}
                    for (int i = 0; i < splitStrsReplaceIndex.Count; i++)
                    {
                        string strNonSym = splitStrs[splitStrsReplaceIndex[i]].Replace("{", "").Replace("}", "");
                        // [i,X] [j,D] [i+2,D]
                        // 支持10进制(D)与16进制(X), 默认输出16进制
                        string[] strSplit;
                        if (strNonSym.Contains(':'))
                            strSplit = strNonSym.Split(":");
                        else
                            strSplit = new string[2] { strNonSym, "X" };

                        // 支持表达式运算

                        ExpressionEvaluator evaluator = new();
                        evaluator.Variables = SerialportCommandScriptVarDict.ToDictionary(pair => pair.Key, pair => (object)(double)pair.Value);
                        var result = evaluator.Evaluate(strSplit[0]);

                        // Double类型转整型
                        if (strSplit[1].ToUpper().Contains('D') || strSplit[1].ToUpper().Contains('X'))
                            splitStrs[splitStrsReplaceIndex[i]] = (Convert.ToInt64(result)).ToString(strSplit[1]);
                        else
                            splitStrs[splitStrsReplaceIndex[i]] = ((double)result).ToString(strSplit[1]);
                    }

                    //Debug.WriteLine("-----");
                    //Debug.WriteLine(string.Join('\n', splitStrs));
                    //Debug.WriteLine("-----");
                    //Debug.WriteLine(string.Join(',', splitStrsReplaceIndex));
                    //Debug.WriteLine("-----");

                    code = string.Concat(splitStrs);

                    // 串口发送
                    // Debug.WriteLine($"[{SerialportCommandPortNameSelectedValue}]:{code}");
                    SerialPortsInstance.WriteString(SerialportCommandPortNameSelectedValue, code);
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(ex.ToString());
                }
            }

            return SerialPortScriptRunStatus.Executed;
        }

        // 获取本行行数
        public int SerialPortCommandScriptGetCurrentLinePointer()
        {
            return SerialportCommandScriptCurruntLineCursor;
        }


        // 设置跳转行
        public void SerialPortCommandScriptGotoLinePointer(int line)
        {
            SerialportCommandScriptCurruntLineCursor = line - 1;
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
                    Filter = "Code File|*.txt;*.task;*.md",
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
                    Filter = "Text File|*.txt|Task Code|*.task|Markdown Code|*.md",
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
                            if (!SerialportCommandScriptIsRun)
                            {
                                SerialPortCommandScriptRunAll();
                                SerialportCommandScriptIsRun = true;
                            }
                            else
                                SerialportCommandScriptIsRun = false;
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

        // 串口命令模块寄存器表加载
        public void SerialPortCommandScriptRegisterLoad()
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
                    var tempDic = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(json, new JsonSerializerSettings() { FloatParseHandling = FloatParseHandling.Decimal });
                    SerialportCommandScriptVarDict.Clear();
                    foreach (var kv in tempDic)
                    {
                        SerialportCommandScriptVarDict.Add(kv.Key, kv.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // 串口命令模块寄存器表保存
        public void SerialPortCommandScriptRegisterSave()
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
                    string json = JsonConvert.SerializeObject(SerialportCommandScriptVarDict, new JsonSerializerSettings() { FloatParseHandling = FloatParseHandling.Decimal });
                    File.WriteAllText(saveFileDialog.FileName, json);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // 串口命令模块寄存器表清空
        public void SerialPortCommandScriptRegisterClear()
        {
            try
            {
                SerialportCommandScriptVarDict.Clear();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// For栈复位
        /// </summary>
        public void SerialPortCommandScriptForStackReset()
        {
            try
            {
                SerialportCommandScriptForStack.Clear();
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

        private CommandBase serialPortCommandScriptRegisterLoadEvent;
        public CommandBase SerialPortCommandScriptRegisterLoadEvent
        {
            get
            {
                if (serialPortCommandScriptRegisterLoadEvent == null)
                {
                    serialPortCommandScriptRegisterLoadEvent = new CommandBase(new Action<object>(param => SerialPortCommandScriptRegisterLoad()));
                }
                return serialPortCommandScriptRegisterLoadEvent;
            }
        }

        private CommandBase serialPortCommandScriptRegisterSaveEvent;
        public CommandBase SerialPortCommandScriptRegisterSaveEvent
        {
            get
            {
                if (serialPortCommandScriptRegisterSaveEvent == null)
                {
                    serialPortCommandScriptRegisterSaveEvent = new CommandBase(new Action<object>(param => SerialPortCommandScriptRegisterSave()));
                }
                return serialPortCommandScriptRegisterSaveEvent;
            }
        }

        private CommandBase serialPortCommandScriptRegisterClearEvent;
        public CommandBase SerialPortCommandScriptRegisterClearEvent
        {
            get
            {
                if (serialPortCommandScriptRegisterClearEvent == null)
                {
                    serialPortCommandScriptRegisterClearEvent = new CommandBase(new Action<object>(param => SerialPortCommandScriptRegisterClear()));
                }
                return serialPortCommandScriptRegisterClearEvent;
            }
        }

        private CommandBase serialPortCommandScriptForStackResetEvent;
        public CommandBase SerialPortCommandScriptForStackResetEvent
        {
            get
            {
                if (serialPortCommandScriptForStackResetEvent == null)
                {
                    serialPortCommandScriptForStackResetEvent = new CommandBase(new Action<object>(param => SerialPortCommandScriptForStackReset()));
                }
                return serialPortCommandScriptForStackResetEvent;
            }
        }

    }

    /// <summary>
    /// 脚本For语句栈信息存储
    /// </summary>
    public class CommandScriptForStatementInfo
    {
        /// <summary>
        /// For 语句开始行地址
        /// </summary>
        public int ForPointer { get; set; }
        /// <summary>
        /// For语句结束行地址
        /// </summary>
        public int EndForPointer { get; set; }
        /// <summary>
        /// For循环变量名
        /// </summary>
        public string Var { get; set; }
        /// <summary>
        /// 初始值
        /// </summary>
        public decimal Begin { get; set; }
        /// <summary>
        /// 结束值
        /// </summary>
        public decimal End { get; set; }
        /// <summary>
        /// 自增值
        /// </summary>
        public decimal Step { get; set; }
    }
}
