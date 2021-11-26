using MeasureApp.Model;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 3458A基本命令状态文本
        private string m3458ABasicConfigCommandLogText;
        public string M3458ABasicConfigCommandLogText
        {
            get => m3458ABasicConfigCommandLogText;
            set
            {
                m3458ABasicConfigCommandLogText = value;
                RaisePropertyChanged(() => M3458ABasicConfigCommandLogText);
            }
        }

        // 3458A设置显示位数
        private ComboBoxItem[] m3458ANdigComboBoxItems = new[] {
            new ComboBoxItem { Content = "3", Tag = "3" } ,
            new ComboBoxItem { Content = "4", Tag = "4" } ,
            new ComboBoxItem { Content = "5", Tag = "5" } ,
            new ComboBoxItem { Content = "6", Tag = "6" } ,
            new ComboBoxItem { Content = "7", Tag = "7" } ,
            new ComboBoxItem { Content = "8", Tag = "8" }};
        public ComboBoxItem[] M3458ANdigComboBoxItems
        {
            get => m3458ANdigComboBoxItems;
            set
            {
                m3458ANdigComboBoxItems = value;
                RaisePropertyChanged(() => M3458ANdigComboBoxItems);
            }
        }

        private string m3458ANdigSelectedValue = "8";
        public string M3458ANdigSelectedValue
        {
            get => m3458ANdigSelectedValue;
            set
            {
                m3458ANdigSelectedValue = value;
                RaisePropertyChanged(() => M3458ANdigSelectedValue);
            }
        }

        // 3458A设置自动校准
        private ComboBoxItem[] m3458AAcalComboBoxItems = new[] {
            new ComboBoxItem { Content = "ALL (11 minutes)", Tag = "ALL" } ,
            new ComboBoxItem { Content = "DCV (1 minute)", Tag = "DCV" } ,
            new ComboBoxItem { Content = "AC (1 minute)", Tag = "AC" } ,
            new ComboBoxItem { Content = "OHMS (10 minutes)", Tag = "OHMS" }};
        public ComboBoxItem[] M3458AAcalComboBoxItems
        {
            get => m3458AAcalComboBoxItems;
            set
            {
                m3458AAcalComboBoxItems = value;
                RaisePropertyChanged(() => M3458AAcalComboBoxItems);
            }
        }

        private string m3458AAcalSelectedValue = "ALL";
        public string M3458AAcalSelectedValue
        {
            get => m3458AAcalSelectedValue;
            set
            {
                m3458AAcalSelectedValue = value;
                RaisePropertyChanged(() => M3458AAcalSelectedValue);
            }
        }

        // 3458A设置量程精度
        private ComboBoxItem[] m3458ASetRangeComboBoxItems = new[] {
            new ComboBoxItem { Content = "Auto", Tag = "AUTO" } ,
            new ComboBoxItem { Content = "100mV", Tag = "0.1" } ,
            new ComboBoxItem { Content = "1V", Tag = "1" } ,
            new ComboBoxItem { Content = "10V", Tag = "10" } ,
            new ComboBoxItem { Content = "100V", Tag = "100" } ,
            new ComboBoxItem { Content = "1kV", Tag = "1000" }};
        public ComboBoxItem[] M3458ASetRangeComboBoxItems
        {
            get => m3458ASetRangeComboBoxItems;
            set
            {
                m3458ASetRangeComboBoxItems = value;
                RaisePropertyChanged(() => M3458ASetRangeComboBoxItems);
            }
        }

        private string m3458ASetRangeSelectedValue = "AUTO";
        public string M3458ASetRangeSelectedValue
        {
            get => m3458ASetRangeSelectedValue;
            set
            {
                m3458ASetRangeSelectedValue = value;
                RaisePropertyChanged(() => M3458ASetRangeSelectedValue);
            }
        }

        private ComboBoxItem[] m3458ASetResolutionComboBoxItems = new[] {
            new ComboBoxItem { Content = "Default", Tag = "DEFAULT" } ,
            new ComboBoxItem { Content = "1nV", Tag = "0.001" } ,
            new ComboBoxItem { Content = "10nV", Tag = "0.01" } ,
            new ComboBoxItem { Content = "100nV", Tag = "0.1" } ,
            new ComboBoxItem { Content = "1uV", Tag = "1" } ,
            new ComboBoxItem { Content = "10uV", Tag = "10" } ,
            new ComboBoxItem { Content = "100uV", Tag = "100" } ,
            new ComboBoxItem { Content = "1mV", Tag = "1000" }};
        public ComboBoxItem[] M3458ASetResolutionComboBoxItems
        {
            get => m3458ASetResolutionComboBoxItems;
            set
            {
                m3458ASetResolutionComboBoxItems = value;
                RaisePropertyChanged(() => M3458ASetResolutionComboBoxItems);
            }
        }

        private string m3458ASetResolutionSelectedValue = "DEFAULT";
        public string M3458ASetResolutionSelectedValue
        {
            get => m3458ASetResolutionSelectedValue;
            set
            {
                m3458ASetResolutionSelectedValue = value;
                RaisePropertyChanged(() => M3458ASetResolutionSelectedValue);
            }
        }

        // 3458A NPLC设置
        private string m3458ASetNplcText = "1";
        public string M3458ASetNplcText
        {
            get => m3458ASetNplcText;
            set
            {
                m3458ASetNplcText = value;
                RaisePropertyChanged(() => M3458ASetNplcText);
            }
        }

        // 3458A 手动测量量程精度设置
        private string m3458AManualMeasureRangeSelectedValue = "AUTO";
        public string M3458AManualMeasureRangeSelectedValue
        {
            get => m3458AManualMeasureRangeSelectedValue;
            set
            {
                m3458AManualMeasureRangeSelectedValue = value;
                RaisePropertyChanged(() => M3458AManualMeasureRangeSelectedValue);
            }
        }

        private string m3458AManualMeasureResolutionSelectedValue = "DEFAULT";
        public string M3458AManualMeasureResolutionSelectedValue
        {
            get => m3458AManualMeasureResolutionSelectedValue;
            set
            {
                m3458AManualMeasureResolutionSelectedValue = value;
                RaisePropertyChanged(() => M3458AManualMeasureResolutionSelectedValue);
            }
        }

        // 3458A 设置NPLC事件
        private CommandBase m3458ASetNPLCEvent;
        public CommandBase M3458ASetNPLCEvent
        {
            get
            {
                if (m3458ASetNPLCEvent == null)
                {
                    m3458ASetNPLCEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            if (Measure3458AInstance.IsOpen)
                            {
                                Measure3458AInstance.SetNPLC(param);
                            }
                            else
                            {
                                _ = MessageBox.Show("GPIB(3458A) is not open.");
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return m3458ASetNPLCEvent;
            }
        }

        // 3458A 同步电压显示开启标志位
        private bool m3458AIsSyncMeasureOpen;
        public bool M3458AIsSyncMeasureOpen

        {
            get => m3458AIsSyncMeasureOpen;
            set
            {
                m3458AIsSyncMeasureOpen = value;
                RaisePropertyChanged(() => M3458AIsSyncMeasureOpen);
            }
        }

        // 3458A 同步电压显示绑定文本
        private string m3458ASyncMeasureText = "--------";
        public string M3458ASyncMeasureText
        {
            get => m3458ASyncMeasureText;
            set
            {
                m3458ASyncMeasureText = value;
                RaisePropertyChanged(() => M3458ASyncMeasureText);
            }
        }

        // 3458A 手动测量电压显示绑定文本
        private string m3458AManualMeasureText = "--------";
        public string M3458AManualMeasureText
        {
            get => m3458AManualMeasureText;
            set
            {
                m3458AManualMeasureText = value;
                RaisePropertyChanged(() => M3458AManualMeasureText);
            }
        }

        // 3458A 同步电压显示事件
        private ManualResetEvent m3458ASyncMeasureManualResetEvent = new(false);
        private CommandBase m3458ASyncMeasureEvent;
        public CommandBase M3458ASyncMeasureEvent
        {
            get
            {
                if (m3458ASyncMeasureEvent == null)
                {
                    m3458ASyncMeasureEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            if (Measure3458AInstance.IsOpen)
                            {
                                if (M3458AIsSyncMeasureOpen)
                                {
                                    M3458AIsSyncMeasureOpen = false;
                                    m3458ASyncMeasureManualResetEvent.Reset();
                                }
                                else
                                {
                                    M3458AIsSyncMeasureOpen = true;
                                    m3458ASyncMeasureManualResetEvent.Set();
                                    _ = Task.Run(() =>
                                    {
                                        try
                                        {
                                            while (true)
                                            {
                                                m3458ASyncMeasureManualResetEvent.WaitOne();
                                                // 3458A Multimeter User's Guide Page 149
                                                decimal DCVDisplay = Measure3458AInstance.ReadDecimal();
                                                M3458ASyncMeasureText = $"{DCVDisplay}";

                                                // 数据自动存储
                                                bool isStorage = (bool)param;
                                                if (isStorage)
                                                {
                                                    DataStorageInstance.AddData(Key3458AString, DCVDisplay);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            _ = MessageBox.Show(ex.ToString());
                                        }
                                    });
                                }
                            }
                            else
                            {
                                _ = MessageBox.Show("GPIB(3458A) is not open.");
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return m3458ASyncMeasureEvent;
            }
        }

        // 3458A 手动测量电压事件
        private CommandBase m3458AManualMeasureEvent;
        public CommandBase M3458AManualMeasureEvent
        {
            get
            {
                if (m3458AManualMeasureEvent == null)
                {
                    m3458AManualMeasureEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            if (Measure3458AInstance.IsOpen)
                            {
                                string setRange = m3458AManualMeasureRangeSelectedValue;
                                string setResolution = M3458AManualMeasureResolutionSelectedValue;
                                decimal setRangeDecimal;
                                decimal setResolutionDecimal;
                                if (setRange == "AUTO")
                                {
                                    setRangeDecimal = -1;
                                    setResolutionDecimal = -1;
                                }
                                else if (setResolution == "DEFAULT")
                                {
                                    setRangeDecimal = Convert.ToDecimal(setRange);
                                    setResolutionDecimal = -1;
                                }
                                else
                                {
                                    setRangeDecimal = Convert.ToDecimal(setRange);
                                    setResolutionDecimal = Convert.ToDecimal(setResolution) / 1000000;
                                }
                                bool isStorage = (bool)param;

                                M3458AManualMeasureText = "Measuring...";
                                _ = Task.Run(() =>
                                {
                                    try
                                    {
                                        M3458AManualMeasureText = Measure3458AInstance.MeasureDCV(setRangeDecimal, setResolutionDecimal).ToString();
                                        // 数据自动存储
                                        if (isStorage)
                                        {
                                            DataStorageInstance.AddData(Key3458AString, Convert.ToDecimal(M3458AManualMeasureText));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _ = MessageBox.Show(ex.ToString());
                                    }
                                });

                            }
                            else
                            {
                                _ = MessageBox.Show("GPIB(3458A) is not open.");
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return m3458AManualMeasureEvent;
            }
        }

        // 3458A发送命令事件
        private CommandBase m3458ABasicConfigEvent;
        public CommandBase M3458ABasicConfigEvent
        {
            get
            {
                if (m3458ABasicConfigEvent == null)
                {
                    m3458ABasicConfigEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            if (Measure3458AInstance.IsOpen)
                            {
                                switch (param as string)
                                {
                                    case "RESET":
                                        Measure3458AInstance.Reset();
                                        M3458ABasicConfigCommandLogText = $"Write: RESET & END";
                                        break;
                                    case "ID":
                                        M3458ABasicConfigCommandLogText = $"Query: ID?\nReturn: {Measure3458AInstance.GetID()}";
                                        break;
                                    case "ERR":
                                        M3458ABasicConfigCommandLogText = $"Query: ERRSTR?\nReturn: {Measure3458AInstance.GetErrorString()}";
                                        break;
                                    case "STB":
                                        M3458ABasicConfigCommandLogText = $"Query: STB?\nReturn: {Measure3458AInstance.ReadStatusByte()}";
                                        break;
                                    case "TEMP":
                                        M3458ABasicConfigCommandLogText = $"Query: TEMP?\nReturn: {Measure3458AInstance.GetTemp()}";
                                        break;
                                    case "LINE":
                                        M3458ABasicConfigCommandLogText = $"Query: LINE?\nReturn: {Measure3458AInstance.GetLineFreq()} Hz";
                                        break;
                                    case "NDIG":
                                        string setNdig = M3458ANdigSelectedValue;
                                        Measure3458AInstance.WriteCommand($"NDIG {setNdig}");
                                        M3458ABasicConfigCommandLogText = $"Write: NDIG {setNdig}";
                                        break;
                                    case "ACAL":
                                        if (MessageBox.Show("需要较长时间，是否继续？", "自动校准确认", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                        {
                                            string acal_param = M3458AAcalSelectedValue;
                                            Measure3458AInstance.WriteCommand("ACAL " + acal_param);
                                            M3458ABasicConfigCommandLogText = $"Write: ACAL {acal_param}";
                                        }
                                        break;
                                    case "RANGE":
                                        // %_resolution = (actual resolution/maximum input) × 100
                                        string setRange = M3458ASetRangeSelectedValue;
                                        string setResolution = M3458ASetResolutionSelectedValue;
                                        string rangeCmd = $"RANGE {setRange}";
                                        if (setRange != "AUTO" && setResolution != "DEFAULT")
                                        {
                                            decimal setRangeDecimal = Convert.ToDecimal(setRange);
                                            decimal setResolutionDecimal = Convert.ToDecimal(setResolution);
                                            rangeCmd += $",{setResolutionDecimal / setRangeDecimal / 10000}";
                                        }
                                        Measure3458AInstance.WriteCommand(rangeCmd);
                                        M3458ABasicConfigCommandLogText = $"Write: {rangeCmd}";
                                        break;
                                    case "NPLC":
                                        string nplcCmd = $"NPLC {M3458ASetNplcText}";
                                        Measure3458AInstance.WriteCommand(nplcCmd);
                                        M3458ABasicConfigCommandLogText = $"Write: {nplcCmd}";
                                        break;
                                    case "RANGE?":
                                        bool isArange = Measure3458AInstance.QueryDecimal("ARANGE?") != 0M;
                                        Measure3458AInstance.WaitForDataAvailable();
                                        decimal readRange = Measure3458AInstance.QueryDecimal("RANGE?");
                                        Measure3458AInstance.WaitForDataAvailable();
                                        decimal readResolution = Measure3458AInstance.QueryDecimal("RES?") * readRange * 10000;
                                        M3458ABasicConfigCommandLogText = $"Query: ARANGE? & RANGE? & RES?\nReturn: {(isArange ? "Auto Range, " + readRange.ToString() + "V" : readRange.ToString() + "V, " + readResolution.ToString() + "uV")}";
                                        break;
                                    case "NPLC?":
                                        M3458ABasicConfigCommandLogText = $"Query: NPLC?\nReturn: {Measure3458AInstance.GetNPLC()} NPLC";
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                            else
                            {
                                _ = MessageBox.Show("GPIB(3458A) is not open.");
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return m3458ABasicConfigEvent;
            }
        }
    }
}
