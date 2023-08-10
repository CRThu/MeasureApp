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
using CarrotProtocolLib.Protocol;
using CarrotProtocolLib.Logger;
using CarrotProtocolLib.Device;
using CarrotProtocolLib.Driver;
using CarrotProtocolLib.Service;

namespace CarrotProtocolCommDemo
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private IDevice device;

        [ObservableProperty]
        private ILogger logger;


        [ObservableProperty]
        private string[] drivers;

        [ObservableProperty]
        private string selectedDriver;

        [ObservableProperty]
        private DeviceInfo[] devicesInfo;

        [ObservableProperty]
        private DeviceInfo selectedDeviceInfo;

        [ObservableProperty]
        private string[] protocolNames;

        [ObservableProperty]
        private string selectedProtocolName;

        public int[] SerialPortBaudRate => SerialPortDriver.SupportedBaudRate;

        [ObservableProperty]
        private int selectedSerialPortBaudRate;

        [ObservableProperty]
        private int[] carrotDataProtocolIds;

        [ObservableProperty]
        private int selectedCarrotDataProtocolId;

        [ObservableProperty]
        private int[] carrotDataProtocolStreamIds;

        [ObservableProperty]
        private int selectedCarrotDataProtocolStreamId;

        [ObservableProperty]
        private string carrotDataProtocolPayloadString = "";


        [ObservableProperty]
        private string inputCode = "";

        [ObservableProperty]
        private string stdOut = "";

        [ObservableProperty]
        private CarrotDataProtocolConfigViewModel cdpCfgVm = new();

        [ObservableProperty]
        private EscapeString escapeString = new EscapeString(@"\\123\11\22\33\fF\f\\\\\s\fss\rrr\nnn\r\n000");


        [ObservableProperty]
        public bool isOpen;


        public MainWindowViewModel()
        {
            Device = new GeneralBufferedDevice();
            Logger = new ProtocolLogger();

            drivers = new string[] { "SerialPort", "FTDI_D2XX" };
            SelectedDriver = "SerialPort";
            DevicesInfoUpdate();
            ProtocolNames = new string[] { "CarrotDataProtocol", "RawAsciiProtocol" };
            SelectedProtocolName = "CarrotDataProtocol";
            selectedSerialPortBaudRate = SerialPortBaudRate.FirstOrDefault();
            CarrotDataProtocolIds = new int[] { 0x30, 0x31, 0x32, 0x33 };
            SelectedCarrotDataProtocolId = CarrotDataProtocolIds.FirstOrDefault();
            CarrotDataProtocolStreamIds = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            SelectedCarrotDataProtocolStreamId = CarrotDataProtocolStreamIds.FirstOrDefault();
            //InputCode = GenHexPkt();
        }

        [RelayCommand]
        private void DevicesInfoUpdate()
        {
            // Interface
            switch (SelectedDriver)
            {
                case "SerialPort":
                    DevicesInfo = SerialPortDriver.GetDevicesInfo();
                    break;
                case "FTDI_D2XX":
                    DevicesInfo = FtdiD2xxDriver.GetDevicesInfo();
                    break;
            }
            SelectedDeviceInfo = DevicesInfo.FirstOrDefault();
        }

        [RelayCommand]
        private void Open()
        {
            try
            {
                if (IsOpen)
                {
                    // Close
                    Debug.WriteLine("Close");
                    Device.Close();
                    IsOpen = false;
                }
                else
                {
                    // Open
                    Debug.WriteLine("Open");

                    string driverName = SelectedDriver switch
                    {
                        "SerialPort" => nameof(SerialPortDriver),
                        "FTDI_D2XX" => nameof(SerialPortDriver),
                        _ => throw new NotImplementedException()
                    };
                    string decodeServiceName = SelectedProtocolName switch
                    {
                        "CarrotDataProtocol" => nameof(CarrotDataProtocolDecodeService),
                        "RawAsciiProtocol" => nameof(RawAsciiProtocolDecodeService),
                        _ => throw new NotImplementedException()
                    };

                    if (Device is not null)
                    {
                        Device.Logger.LoggerUpdate -= Logger_LoggerUpdate;

                    }
                    Device = DeviceFactory.Create(
                        nameof(GeneralBufferedDevice),
                        driverName, //nameof(SerialPortDriver),
                        nameof(ProtocolLogger),
                        nameof(DeviceDataReceiveService),
                        decodeServiceName); // nameof(CarrotDataProtocolDecodeService));
                    Device.Logger.LoggerUpdate += Logger_LoggerUpdate;

                    if (Device.Driver is SerialPortDriver)
                    {
                        (Device.Driver as SerialPortDriver)!.SetDriver(SelectedDeviceInfo.Name, SelectedSerialPortBaudRate, 8, 1, "None");
                    }
                    else if (Device.Driver is FtdiD2xxDriver)
                    {
                        (Device.Driver as FtdiD2xxDriver)!.SetDriver(SelectedDeviceInfo.Name);
                    }
                    else
                        throw new NotImplementedException($"Cannot SetDriver, Class = {Device.Driver.GetType()}");

                    Device.Open();
                    IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ProtocolParseErrorCallback(Exception ex)
        {
            MessageBox.Show(ex.ToString());
            Device.Close();
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
                CarrotDataProtocolFrame carrotDataProtocol = new(SelectedCarrotDataProtocolId, SelectedCarrotDataProtocolStreamId, CarrotDataProtocolPayloadString);
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
                byte[] b = BytesEx.HexStringToBytes(InputCode);
                Device.Write(b, 0, b.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 发送RawAscii协议数据
        /// </summary>
        [RelayCommand]
        private void RawAsciiProtocolSend()
        {
            try
            {
                RawAsciiProtocolFrame rec = new(EscapeString.RawBytes, 0, EscapeString.RawBytes.Length);
                Debug.WriteLine($"Send {nameof(RawAsciiProtocolFrame)}: {rec.FrameBytes.BytesToHexString()}");
                Device.Write(rec);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
