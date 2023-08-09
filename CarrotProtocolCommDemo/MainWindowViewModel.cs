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
using CarrotProtocolLib.Logger;
using CarrotProtocolLib.Device;
using CarrotProtocolLib.Protocol;
using CarrotProtocolLib.Driver;
using CarrotProtocolLib.Service;
using static System.Net.Mime.MediaTypeNames;

namespace CarrotProtocolCommDemo
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public IDevice Device { get; set; }

        public ILogger Logger { get; set; }


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
        private EscapeString escapeString = new EscapeString(@"12345\01\02\\\03abc\\\");

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
                CarrotDataProtocolRecord carrotDataProtocol = new(SelectedCarrotDataProtocolId, SelectedCarrotDataProtocolStreamId, CarrotDataProtocolPayloadString);
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

        [RelayCommand]
        private void AsciiProtocolPayloadChanged()
        {
            //asciiProtocolRecord = new RawAsciiProtocolRecord(AsciiProtocolPayloadString);
            //AsciiProtocolFrameBytes = asciiProtocolRecord.Bytes;
        }

        [RelayCommand]
        private void AsciiProtocolSend()
        {
            try
            {
                RawAsciiProtocolRecord rec = new(EscapeString.TextString);
                Debug.WriteLine($"RawAsciiProtocolRecord: {rec.Bytes.BytesToHexString()}");
                Device.Write(rec.Bytes, 0, rec.Bytes.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
