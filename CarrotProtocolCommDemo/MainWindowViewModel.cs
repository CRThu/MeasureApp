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
        private RawAsciiProtocolRecord asciiProtocolRecord = new RawAsciiProtocolRecord("");

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
            drivers = new string[] { "SerialPort", "FTDI_D2xx" };
            SelectedDriver = "SerialPort";
            DevicesInfoUpdate();
            ProtocolNames = new string[] { "CarrotDataProtocol", "RawAsciiProtocol" };
            SelectedProtocolName = "CarrotDataProtocol";
            CarrotProtocols = new int[] { 0x30, 0x31, 0x32, 0x33 };
            SelectedCarrotProtocol = CarrotProtocols.FirstOrDefault();
            CarrotProtocolStreamIds = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            SelectedCarrotProtocolStreamId = CarrotProtocolStreamIds.FirstOrDefault();
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
                Device = DeviceFactory.Create(
                    nameof(GeneralBufferedDevice),
                    nameof(SerialPortDriver),
                    nameof(ProtocolLogger),
                    nameof(DeviceDataReceiveService),
                    nameof(CarrotDataProtocolDecodeService));
                Device.Open();
                //MessageBox.Show("Open");
                if (Device.IsOpen)
                {
                }
                else
                {
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
                //MessageBox.Show(test);
                //MessageBox.Show("Send");
                Device.Write(asciiProtocolRecord.Bytes, 0, asciiProtocolRecord.Bytes.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
