using MeasureApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                            if (param is string)
                            {
                                gpibDeviceConnectStatusText = $"No Device Connected.";
                                Measure3458AInstance.Dispose();
                                gpibDeviceConnectStatusText = $"{Measure3458AInstance.Open(param as string)} Connected.";
                                Measure3458AInstance.Timeout = Properties.Settings.Default.GPIBTimeout;
                            }
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
                            // TODO 115200
                            if (!SerialPortsInstance.Open(portName, 115200))
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
    }
}
