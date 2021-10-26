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

        // 未成功实现
        // ComboBox数据源更新事件
        private CommandBase comboBoxSourceUpdatedEvent;
        public CommandBase ComboBoxSourceUpdatedEvent
        {
            get
            {
                if (comboBoxSourceUpdatedEvent == null)
                {
                    comboBoxSourceUpdatedEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            MessageBox.Show("UPDATED");
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return comboBoxSourceUpdatedEvent;
            }
        }

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
        private CommandBase gpibDeviceSearchEvent;
        public CommandBase GpibDeviceSearchEvent
        {
            get
            {
                if (gpibDeviceSearchEvent == null)
                {
                    gpibDeviceSearchEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            GpibDevicesName.Clear();
                            GPIB.SearchDevices("GPIB?*INSTR").ToList().ForEach(dev => GpibDevicesName.Add(dev));
                            if (GpibDevicesSelectedName is null)
                                GpibDevicesSelectedName = GpibDevicesName.First();
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
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
        private ObservableCollection<ComboBoxItem> serialportDevicesNameComboBoxItems = new();
        public ObservableCollection<ComboBoxItem> SerialportDevicesNameComboBoxItems
        {
            get => serialportDevicesNameComboBoxItems;
            set
            {
                serialportDevicesNameComboBoxItems = value;
                RaisePropertyChanged(() => SerialportDevicesNameComboBoxItems);
            }
        }

        // 选中串口设备
        private ComboBoxItem serialportDevicesNameSelectItem = new();
        public ComboBoxItem SerialportDevicesNameSelectItem
        {
            get => serialportDevicesNameSelectItem;
            set
            {
                serialportDevicesNameSelectItem = value;
                RaisePropertyChanged(() => SerialportDevicesNameSelectItem);
            }
        }

        // 串口波特率
        private string serialportDeviceBaudRateSelectedValue = "9600";
        public string SerialportDeviceBaudRateSelectedValue
        {
            get => serialportDeviceBaudRateSelectedValue;
            set
            {
                serialportDeviceBaudRateSelectedValue = value;
                RaisePropertyChanged(() => SerialportDeviceBaudRateSelectedValue);
            }
        }


        // 串口校验
        private string[] serialportDeviceParityList = Enum.GetNames(typeof(Parity));
        public string[] SerialportDeviceParityList
        {
            get => serialportDeviceParityList;
            set
            {
                serialportDeviceParityList = value;
                RaisePropertyChanged(() => SerialportDeviceParityList);
            }
        }

        private string serialportDeviceParitySelectItem = Enum.GetName(typeof(Parity), Parity.None);
        public string SerialportDeviceParitySelectItem
        {
            get => serialportDeviceParitySelectItem;
            set
            {
                serialportDeviceParitySelectItem = value;
                RaisePropertyChanged(() => SerialportDeviceParitySelectItem);
            }
        }

        // 串口数据位
        private int[] serialportDeviceDataBitsList = new[] { 5, 6, 7, 8 };
        public int[] SerialportDeviceDataBitsList
        {
            get => serialportDeviceDataBitsList;
            set
            {
                serialportDeviceDataBitsList = value;
                RaisePropertyChanged(() => SerialportDeviceDataBitsList);
            }
        }

        private int serialportDeviceDataBitsSelectItem = 8;
        public int SerialportDeviceDataBitsSelectItem
        {
            get => serialportDeviceDataBitsSelectItem;
            set
            {
                serialportDeviceDataBitsSelectItem = value;
                RaisePropertyChanged(() => SerialportDeviceDataBitsSelectItem);
            }
        }

        // 串口停止位
        private float[] serialportDeviceStopBitsList = new[] { 1.0F, 1.5F, 2.0F };
        public float[] SerialportDeviceStopBitsList
        {
            get => serialportDeviceStopBitsList;
            set
            {
                serialportDeviceStopBitsList = value;
                RaisePropertyChanged(() => SerialportDeviceStopBitsList);
            }
        }

        private float serialportDeviceStopBitsSelectItem = 1.0F;
        public float SerialportDeviceStopBitsSelectItem
        {
            get => serialportDeviceStopBitsSelectItem;
            set
            {
                serialportDeviceStopBitsSelectItem = value;
                RaisePropertyChanged(() => SerialportDeviceStopBitsSelectItem);
            }
        }

        // 搜索串口设备事件
        private CommandBase serialPortDeviceSearchEvent;
        public CommandBase SerialPortDeviceSearchEvent
        {
            get
            {
                if (serialPortDeviceSearchEvent == null)
                {
                    serialPortDeviceSearchEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            serialportDevicesNameComboBoxItems.Clear();
                            string[] portList = SerialPort.GetPortNames();
                            string[] portDescriptionList = HardwareInfoUtil.GetSerialPortFullName();
                            for (int i = 0; i < portList.Length; ++i)
                            {
                                serialportDevicesNameComboBoxItems.Add(new()
                                {
                                    Content = $"{portList[i]}|{portDescriptionList.Where(str => str.Contains(portList[i])).FirstOrDefault() ?? ""}",
                                    Tag = portList[i]
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return serialPortDeviceSearchEvent;
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
                            string portName = SerialportDevicesNameSelectItem.Tag as string;
                            int baudRate = Convert.ToInt32(SerialportDeviceBaudRateSelectedValue);
                            Parity parity = (Parity)Enum.Parse(typeof(Parity), SerialportDeviceParitySelectItem);
                            int dataBits = serialportDeviceDataBitsSelectItem;
                            StopBits stopBits = serialportDeviceStopBitsSelectItem switch
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
                            string portName = SerialportDevicesNameSelectItem.Tag as string;
                            if (!SerialPortsInstance.Close(portName))
                            {
                                _ = MessageBox.Show("串口已被关闭.");
                            }
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
    }
}
