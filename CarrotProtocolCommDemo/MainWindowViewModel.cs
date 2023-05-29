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
using CarrotProtocolLib.Driver;
using CarrotProtocolLib.Interface;

namespace CarrotProtocolCommDemo
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public IDevice Device { get; set; }
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
        private bool isOpen = false;

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
                    Devices = new DeviceInfo[] { };
                    break;
            }
            SelectedDevice = Devices.FirstOrDefault();
        }

        [RelayCommand]
        private void Open()
        {
            try
            {
                //MessageBox.Show("Open");
                if (Device is null)
                {
                    Device = new SerialPortDevice();
                }

                if (!Device.IsOpen)
                {
                    ((SerialPortDevice)Device).SetDevice(SelectedDevice.Name, 115200, 8, 1, "None");
                    Logger = new ProtocolLogger(Logger_LoggerUpdate);
                    Protocol = new CarrotDataProtocol(Device, Logger, Protocol_ReceiveError);
                    Protocol.Start();
                }
                else
                {
                    Protocol.Stop();
                }
                IsOpen = Device.IsOpen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Protocol_ReceiveError(Exception ex)
        {
            MessageBox.Show(ex.ToString());
            Protocol.Stop();
            IsOpen = Device.IsOpen;
        }

        [RelayCommand]
        private void PacketParamsChanged()
        {
            try
            {
                CarrotDataProtocolLog carrotDataProtocol = new(SelectedCarrotProtocol, SelectedCarrotProtocolStreamId, PayloadString);
                byte[] bytes = carrotDataProtocol.ToBytes();
                InputCode = BytesEx.BytesToHexString(bytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
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
    }
}
