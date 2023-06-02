using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.IO.Ports;
using System.Diagnostics;
using CarrotProtocolLib.Util;
using CarrotProtocolLib.Impl;
using CarrotProtocolLib.Interface;
using CarrotProtocolLib.Logger;
using CarrotProtocolLib.Device;
using CarrotProtocolLib.Protocol;

namespace CarrotProtocolCommDemo
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private IDevice device;
        public IDevice Device
        {
            get
            {
                return device;
            }
            set
            {
                if (device is not null)
                    device.InternalPropertyChanged -= View_Update;
                device = value;
                if (device is not null)
                    device.InternalPropertyChanged += View_Update;
            }
        }

        public ILogger Logger { get; set; }
        public IProtocol Protocol { get; set; }


        [ObservableProperty]
        private InterfaceType[] interfaces;

        [ObservableProperty]
        private InterfaceType selectedInterface;

        [ObservableProperty]
        private DeviceInfo[] devices;

        [ObservableProperty]
        private DeviceInfo selectedDevice;

        [ObservableProperty]
        private string[] protocolNames;

        [ObservableProperty]
        private string selectedProtocolName;

        [ObservableProperty]
        private int[] carrotProtocols;

        [ObservableProperty]
        private int selectedCarrotProtocol;

        [ObservableProperty]
        private int[] carrotProtocolStreamIds;

        [ObservableProperty]
        private int selectedCarrotProtocolStreamId;

        [ObservableProperty]
        private string payloadString = "";


        [ObservableProperty]
        private string inputCode = "";

        [ObservableProperty]
        private string stdOut = "";

        [ObservableProperty]
        private AsciiProtocolRecord asciiProtocolRecord = new AsciiProtocolRecord("");

        [ObservableProperty]
        private string asciiProtocolPayloadString = "";
        [ObservableProperty]
        private byte[] asciiProtocolFrameBytes = Array.Empty<byte>();

        [ObservableProperty]
        public int receivedByteCount;
        [ObservableProperty]
        public int sentByteCount;

        [ObservableProperty]
        public bool isOpen;


        public MainWindowViewModel()
        {
            Interfaces = new InterfaceType[] { InterfaceType.SerialPort, InterfaceType.FTDI_D2XX };
            SelectedInterface = InterfaceType.SerialPort;
            InterfaceChanged();
            ProtocolNames = new string[] { "CarrotDataProtocol", "AsciiProtocol", "UartBinaryProtocol" };
            SelectedProtocolName = "CarrotDataProtocol";
            CarrotProtocols = new int[] { 0x30, 0x31, 0x32, 0x33 };
            SelectedCarrotProtocol = CarrotProtocols.FirstOrDefault();
            CarrotProtocolStreamIds = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            SelectedCarrotProtocolStreamId = CarrotProtocolStreamIds.FirstOrDefault();

            Device = EmptyDevice.EmptyDeviceInstance;
            // Device = new SerialPortDevice();
            //InputCode = GenHexPkt();
        }

        [RelayCommand]
        private void InterfaceChanged()
        {
            switch (SelectedInterface)
            {
                case InterfaceType.SerialPort:
                    Devices = SerialPortDevice.GetDevicesInfo();
                    break;
                case InterfaceType.FTDI_D2XX:
                    Devices = FtdiD2xxDevice.GetDevicesInfo();
                    break;
            }
            SelectedDevice = Devices.FirstOrDefault();

            //Device ??= SelectedInterface switch
            //{
            //    InterfaceType.SerialPort => new SerialPortDevice(),
            //    InterfaceType.FTDI_D2XX => throw new NotImplementedException(),
            //    _ => throw new NotImplementedException(),
            //};
        }

        [RelayCommand]
        private void Open()
        {
            try
            {
                //MessageBox.Show("Open");
                if (Device.IsOpen)
                {
                    // 若设备开启则关闭
                    Protocol.Stop();
                }
                else
                {
                    // 若设备关闭或为空则新建实例
                    Device = SelectedInterface switch
                    {
                        InterfaceType.SerialPort => new SerialPortDevice(),
                        InterfaceType.FTDI_D2XX => new FtdiD2xxDevice(),
                        _ => throw new NotImplementedException(),
                    };

                    // 硬件配置
                    switch (SelectedInterface)
                    {
                        case InterfaceType.SerialPort:
                            ((SerialPortDevice)Device).SetDevice(SelectedDevice.Name, 115200, 8, 1, "None");
                            break;
                        case InterfaceType.FTDI_D2XX:
                            ((FtdiD2xxDevice)Device).SetDevice(SelectedDevice.Name);
                            break;
                    }

                    // 记录器配置
                    Logger = new ProtocolLogger(Logger_LoggerUpdate);

                    // 解析协议配置
                    Protocol = SelectedProtocolName switch
                    {
                        "CarrotDataProtocol" => new CarrotDataProtocol(Device, Logger, ProtocolParseErrorCallback),
                        "AsciiProtocol" => new AsciiProtocol(Device, Logger, ProtocolParseErrorCallback),
                        _ => throw new NotImplementedException(),
                    };
                    Protocol.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void View_Update(string name, dynamic value)
        {
            switch (name)
            {
                case "IsOpen":
                    IsOpen = value;
                    break;
                case "ReceivedByteCount":
                    ReceivedByteCount = value;
                    break;
                case "SentByteCount":
                    SentByteCount = value;
                    break;
                default:
                    break;
            }
        }

        private void ProtocolParseErrorCallback(Exception ex)
        {
            MessageBox.Show(ex.ToString());
            Protocol.Stop();
        }

        private void Logger_LoggerUpdate(ILoggerRecord log, LoggerUpdateEvent e)
        {
            lock (StdOut)
            {
                if (StdOut.Length > 1024)
                    StdOut = "...\n";
                StdOut += log.ToString() + Environment.NewLine;
            }
        }


        [RelayCommand]
        private void PacketParamsChanged()
        {
            try
            {
                CarrotDataProtocolRecord carrotDataProtocol = new(SelectedCarrotProtocol, SelectedCarrotProtocolStreamId, PayloadString);
                byte[] bytes = carrotDataProtocol.ToBytes();
                InputCode = BytesEx.BytesToHexString(bytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        private void Send()
        {
            try
            {
                //MessageBox.Show("Send");
                Protocol.Send(BytesEx.HexStringToBytes(InputCode));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        private void AsciiProtocolPayloadChanged()
        {
            //asciiProtocolRecord = new AsciiProtocolRecord(AsciiProtocolPayloadString);
            //AsciiProtocolFrameBytes = asciiProtocolRecord.Bytes;
        }

        [RelayCommand]
        private void AsciiProtocolSend()
        {
            try
            {
                string teststr = @"12345\01\02\\\03abc\\\";
                byte[] testarr = AsciiString.AsciiString2Bytes(teststr);
                string test = $"{BytesEx.BytesToHexString(AsciiString.AsciiString2Bytes(teststr))}\n" +
                    $"{AsciiString.Bytes2AsciiString(testarr)}";
                MessageBox.Show(test);
                //MessageBox.Show("Send");
                Protocol.Send(asciiProtocolRecord.Bytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
