using MeasureApp.Model;
using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 可用GPIB设备地址
        private ObservableCollection<string> gpibDevicesName = new();
        public ObservableCollection<string> GpibDevicesName
        {
            get => gpibDevicesName;
            set
            {
                gpibDevicesName = value;
                RaisePropertyChanged(() => GpibDevicesName);
            }
        }

        // 选中的GPIB设备地址
        private string gpibDevicesSelectedName;
        public string GpibDevicesSelectedName
        {
            get => gpibDevicesSelectedName;
            set
            {
                gpibDevicesSelectedName = value;
                RaisePropertyChanged(() => GpibDevicesSelectedName);
            }
        }

        // 打开GPIB的设备ID读取
        private string gpibDeviceConnectStatusText = $"No Device Connected.";
        public string GpibDeviceConnectStatusText
        {
            get => gpibDeviceConnectStatusText;
            set
            {
                gpibDeviceConnectStatusText = value;
                RaisePropertyChanged(() => GpibDeviceConnectStatusText);
            }
        }


        // 搜索GPIB设备事件
        public void GpibSearchDevice()
        {
            try
            {
                GpibDevicesName.Clear();
                GPIB.SearchDevices("GPIB?*INSTR").ToList().ForEach(dev => GpibDevicesName.Add(dev));
                if (GpibDevicesName.Count != 0 && GpibDevicesSelectedName is null)
                {
                    GpibDevicesSelectedName = GpibDevicesName.First();
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private CommandBase gpibDeviceSearchEvent;
        public CommandBase GpibDeviceSearchEvent
        {
            get
            {
                if (gpibDeviceSearchEvent == null)
                {
                    gpibDeviceSearchEvent = new CommandBase(new Action<object>(param => GpibSearchDevice()));
                }
                return gpibDeviceSearchEvent;
            }
        }

        // 打开GPIB设备事件
        private CommandBase gpibDeviceOpenEvent;
        public CommandBase GpibDeviceOpenEvent
        {
            get
            {
                if (gpibDeviceOpenEvent == null)
                {
                    gpibDeviceOpenEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            gpibDeviceConnectStatusText = $"No Device Connected.";
                            Measure3458AInstance.Dispose();
                            GpibDeviceConnectStatusText = $"{Measure3458AInstance.Open(GpibDevicesSelectedName)} Connected.";
                            Measure3458AInstance.Timeout = Properties.Settings.Default.GPIBTimeout;
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return gpibDeviceOpenEvent;
            }
        }

        // 可用串口设备
        private ObservableCollection<string> serialportDevicesNameComboBoxItems = new();
        public ObservableCollection<string> SerialportDevicesNameComboBoxItems
        {
            get => serialportDevicesNameComboBoxItems;
            set
            {
                serialportDevicesNameComboBoxItems = value;
                RaisePropertyChanged(() => SerialportDevicesNameComboBoxItems);
            }
        }

        // 选中串口设备
        private string serialportDevicesNameSelectedValue;
        public string SerialportDevicesNameSelectedValue
        {
            get => serialportDevicesNameSelectedValue;
            set
            {
                serialportDevicesNameSelectedValue = value;
                RaisePropertyChanged(() => SerialportDevicesNameSelectedValue);
            }
        }

        // 串口波特率
        public int[] SerialportDeviceBaudRateList { get; set; } = new[] { 9600, 38400, 115200, 921600 };
        public int SerialportDeviceBaudRate
        {
            get => Properties.Settings.Default.DefaultSerialPortBaudRate;
            set
            {
                Properties.Settings.Default.DefaultSerialPortBaudRate = value;
                RaisePropertyChanged(() => SerialportDeviceBaudRate);
            }
        }


        // 串口校验
        public static string[] SerialportDeviceParityList { get; set; } = Enum.GetNames(typeof(Parity));

        // Enum.GetName(typeof(Parity), Parity.None);
        public string SerialportDeviceParitySelectedValue
        {
            get => Properties.Settings.Default.DefaultSerialPortParity;
            set
            {
                Properties.Settings.Default.DefaultSerialPortParity = value;
                RaisePropertyChanged(() => SerialportDeviceParitySelectedValue);
            }
        }

        // 串口数据位
        public int[] SerialportDeviceDataBitsList { get; set; } = new[] { 5, 6, 7, 8 };

        private int serialportDeviceDataBitsSelectItem = Properties.Settings.Default.DefaultSerialPortDataBits;
        public int SerialportDeviceDataBitsSelectedValue
        {
            get => serialportDeviceDataBitsSelectItem;
            set
            {
                serialportDeviceDataBitsSelectItem = value;
                RaisePropertyChanged(() => SerialportDeviceDataBitsSelectedValue);
            }
        }

        // 串口停止位
        public float[] SerialportDeviceStopBitsList { get; set; } = new[] { 1.0F, 1.5F, 2.0F };

        private float serialportDeviceStopBitsSelectedValue = Properties.Settings.Default.DefaultSerialPortStopBits;
        public float SerialportDeviceStopBitsSelectedValue
        {
            get => serialportDeviceStopBitsSelectedValue;
            set
            {
                serialportDeviceStopBitsSelectedValue = value;
                RaisePropertyChanged(() => SerialportDeviceStopBitsSelectedValue);
            }
        }

        // 搜索串口设备事件
        public void SerialPortSearchDevice()
        {
            try
            {
                SerialportDevicesNameComboBoxItems.Clear();
                string[] portList = SerialPort.GetPortNames();
                string[] portDescriptionList = HardwareInfoUtil.GetSerialPortFullName();
                for (int i = 0; i < portList.Length; ++i)
                {
                    SerialportDevicesNameComboBoxItems.Add($"{portList[i]}|{portDescriptionList.Where(str => str.Contains(portList[i])).FirstOrDefault() ?? ""}");
                }

                if (SerialportDevicesNameComboBoxItems.Count != 0 && SerialportDevicesNameSelectedValue is null)
                {
                    SerialportDevicesNameSelectedValue = SerialportDevicesNameComboBoxItems.First();
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // 打开串口设备事件
        private CommandBase serialPortDeviceOpenEvent;
        public CommandBase SerialPortDeviceOpenEvent
        {
            get
            {
                if (serialPortDeviceOpenEvent == null)
                {
                    serialPortDeviceOpenEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            string portName = SerialportDevicesNameSelectedValue.Split('|')[0];
                            int baudRate = SerialportDeviceBaudRate;
                            Parity parity = (Parity)Enum.Parse(typeof(Parity), SerialportDeviceParitySelectedValue);
                            int dataBits = serialportDeviceDataBitsSelectItem;
                            StopBits stopBits = serialportDeviceStopBitsSelectedValue switch
                            {
                                1.0F => StopBits.One,
                                1.5F => StopBits.OnePointFive,
                                2.0F => StopBits.Two,
                                _ => throw new NotImplementedException(),
                            };
                            if (!SerialPortsInstance.Open(portName, baudRate, parity, dataBits, stopBits))
                            {
                                _ = MessageBox.Show("串口已被打开.");
                            }

                            // 更新串口默认选择
                            if (SerialPortsInstance.SerialPortNames.Any() && SerialportDebugPortNameSelectedValue is null)
                                SerialportDebugPortNameSelectedValue = SerialPortsInstance.SerialPortsDict.Keys.First();
                            if (SerialPortsInstance.SerialPortNames.Any() && SerialPortSendCmdSerialPortNameSelectedValue is null)
                                SerialPortSendCmdSerialPortNameSelectedValue = SerialPortsInstance.SerialPortsDict.Keys.First();
                            if (SerialPortsInstance.SerialPortNames.Any() && SerialPortRecvDataSerialPortNameSelectedValue is null)
                                SerialPortRecvDataSerialPortNameSelectedValue = SerialPortsInstance.SerialPortsDict.Keys.First();
                            if (SerialPortsInstance.SerialPortNames.Any() && SerialportCommandPortNameSelectedValue is null)
                                SerialportCommandPortNameSelectedValue = SerialPortsInstance.SerialPortsDict.Keys.First();

                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return serialPortDeviceOpenEvent;
            }
        }

        // 关闭串口设备事件
        private CommandBase serialPortDeviceCloseEvent;
        public CommandBase SerialPortDeviceCloseEvent
        {
            get
            {
                if (serialPortDeviceCloseEvent == null)
                {
                    serialPortDeviceCloseEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            string portName = SerialportDevicesNameSelectedValue.Split('|')[0];
                            if (!SerialPortsInstance.Close(portName))
                            {
                                _ = MessageBox.Show("串口已被关闭.");
                            }

                            // 更新串口默认选择
                            if (SerialPortsInstance.SerialPortNames.Any() && SerialportDebugPortNameSelectedValue is null)
                                SerialportDebugPortNameSelectedValue = SerialPortsInstance.SerialPortsDict.Keys.First();
                            if (SerialPortsInstance.SerialPortNames.Any() && SerialPortSendCmdSerialPortNameSelectedValue is null)
                                SerialPortSendCmdSerialPortNameSelectedValue = SerialPortsInstance.SerialPortsDict.Keys.First();
                            if (SerialPortsInstance.SerialPortNames.Any() && SerialPortRecvDataSerialPortNameSelectedValue is null)
                                SerialPortRecvDataSerialPortNameSelectedValue = SerialPortsInstance.SerialPortsDict.Keys.First();
                            if (SerialPortsInstance.SerialPortNames.Any() && SerialportCommandPortNameSelectedValue is null)
                                SerialportCommandPortNameSelectedValue = SerialPortsInstance.SerialPortsDict.Keys.First();
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return serialPortDeviceCloseEvent;
            }
        }

        // GPIB调试写指令TextBox
        private string gPIBDebugWriteCommandText = $"";
        public string GPIBDebugWriteCommandText
        {
            get => gPIBDebugWriteCommandText;
            set
            {
                gPIBDebugWriteCommandText = value;
                RaisePropertyChanged(() => GPIBDebugWriteCommandText);
            }
        }

        // GPIB调试读指令TextBox
        private string gPIBDebugReadCommandText = $"";
        public string GPIBDebugReadCommandText
        {
            get => gPIBDebugReadCommandText;
            set
            {
                gPIBDebugReadCommandText = value;
                RaisePropertyChanged(() => GPIBDebugReadCommandText);
            }
        }

        // GPIB调试查询事件
        private CommandBase gPIBDebugCommandEvent;
        public CommandBase GPIBDebugCommandEvent
        {
            get
            {
                if (gPIBDebugCommandEvent == null)
                {
                    gPIBDebugCommandEvent = new CommandBase(new Action<object>(param =>
                    {
                        if (Measure3458AInstance.IsOpen)
                        {
                            try
                            {
                                switch (param as string)
                                {
                                    case "Query":
                                        GPIBDebugReadCommandText = Measure3458AInstance.QueryCommand(GPIBDebugWriteCommandText);
                                        break;
                                    case "Write":
                                        Measure3458AInstance.WriteCommand(GPIBDebugWriteCommandText);
                                        break;
                                    case "Read":
                                        GPIBDebugReadCommandText = Measure3458AInstance.ReadString();
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                            catch (Exception ex)
                            {
                                _ = MessageBox.Show(ex.ToString());
                            }
                        }
                        else
                        {
                            _ = MessageBox.Show("GPIB is not open.");
                        }
                    }));
                }
                return gPIBDebugCommandEvent;
            }
        }

        // 串口调试模块通信指定串口绑定
        private string serialportDebugPortNameSelectedValue;
        public string SerialportDebugPortNameSelectedValue
        {
            get => serialportDebugPortNameSelectedValue;
            set
            {
                serialportDebugPortNameSelectedValue = value;
                RaisePropertyChanged(() => SerialportDebugPortNameSelectedValue);
            }
        }

        // 串口调试模块16进制选择绑定
        private bool serialPortDebugWriteIsHex;
        public bool SerialPortDebugWriteIsHex
        {
            get => serialPortDebugWriteIsHex;
            set
            {
                serialPortDebugWriteIsHex = value;
                RaisePropertyChanged(() => SerialPortDebugWriteIsHex);
            }
        }

        private bool serialPortDebugReadIsHex;
        public bool SerialPortDebugReadIsHex
        {
            get => serialPortDebugReadIsHex;
            set
            {
                serialPortDebugReadIsHex = value;
                RaisePropertyChanged(() => SerialPortDebugReadIsHex);
            }
        }

        // 串口调试模块收发文本框绑定
        private string serialPortDebugWriteCommandText;
        public string SerialPortDebugWriteCommandText
        {
            get => serialPortDebugWriteCommandText;
            set
            {
                serialPortDebugWriteCommandText = value;
                RaisePropertyChanged(() => SerialPortDebugWriteCommandText);
            }
        }

        private string serialPortDebugReadCommandText;
        public string SerialPortDebugReadCommandText
        {
            get => serialPortDebugReadCommandText;
            set
            {
                serialPortDebugReadCommandText = value;
                RaisePropertyChanged(() => SerialPortDebugReadCommandText);
            }
        }


        // 串口调试模块发送命令事件
        private CommandBase serialPortDebugWriteCmdEvent;
        public CommandBase SerialPortDebugWriteCmdEvent
        {
            get
            {
                if (serialPortDebugWriteCmdEvent == null)
                {
                    serialPortDebugWriteCmdEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            if (SerialPortDebugWriteIsHex)
                            {
                                SerialPortsInstance.WriteBytes(SerialportDebugPortNameSelectedValue, Utility.Hex2Bytes(SerialPortDebugWriteCommandText));
                            }
                            else
                            {
                                SerialPortsInstance.WriteString(SerialportDebugPortNameSelectedValue, SerialPortDebugWriteCommandText);
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return serialPortDebugWriteCmdEvent;
            }
        }

        // 串口调试模块接收命令事件
        private CommandBase serialPortDebugReadCmdEvent;
        public CommandBase SerialPortDebugReadCmdEvent
        {
            get
            {
                if (serialPortDebugReadCmdEvent == null)
                {
                    serialPortDebugReadCmdEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            SerialPortDebugReadCommandText = SerialPortDebugReadIsHex
                                ? Utility.Bytes2Hex(SerialPortsInstance.ReadExistingBytes(SerialportDebugPortNameSelectedValue))
                                : SerialPortsInstance.ReadExistingString(SerialportDebugPortNameSelectedValue);
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return serialPortDebugReadCmdEvent;
            }
        }

        // CommandBase
        private CommandBase serialPortDeviceSearchEvent;
        public CommandBase SerialPortDeviceSearchEvent
        {
            get
            {
                if (serialPortDeviceSearchEvent == null)
                {
                    serialPortDeviceSearchEvent = new CommandBase(new Action<object>(param => SerialPortSearchDevice()));
                }
                return serialPortDeviceSearchEvent;
            }
        }

    }
}
