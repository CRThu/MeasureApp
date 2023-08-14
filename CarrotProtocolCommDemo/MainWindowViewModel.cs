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
using CarrotProtocolCommDemo.ViewModel;

namespace CarrotProtocolCommDemo
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private IDevice device;

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
        private string stdOut = "";

        [ObservableProperty]
        private CarrotDataProtocolConfigViewModel cdpCfgVm = new();

        [ObservableProperty]
        private RawAsciiProtocolConfigViewModel rapCfgVm = new RawAsciiProtocolConfigViewModel(@"\\123\11\22\33\fF\f\\\\\s\fss\rrr\nnn\r\n000");


        [ObservableProperty]
        public bool isOpen;


        public MainWindowViewModel()
        {
            Device = new GeneralBufferedDevice();

            drivers = new string[] { "SerialPort", "FTDI_D2XX" };
            SelectedDriver = "SerialPort";
            DevicesInfoUpdate();
            ProtocolNames = new string[] { "CarrotDataProtocol", "RawAsciiProtocol" };
            SelectedProtocolName = "CarrotDataProtocol";
            selectedSerialPortBaudRate = SerialPortBaudRate.FirstOrDefault();
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

                    Device = DeviceFactory.Create(
                        "GeneralBufferedDevice",
                        SelectedDriver,
                        "ProtocolLogger",
                        "DataReceive",
                        SelectedProtocolName);

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

        [RelayCommand]
        private void CarrotDataProtocolSend()
        {
            try
            {
                CarrotDataProtocolFrame rec = CdpCfgVm.Frame;
                Debug.WriteLine($"Send {nameof(CarrotDataProtocolFrame)}: {rec.FrameBytes.BytesToHexString()}");
                Device.Write(rec);
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
                RawAsciiProtocolFrame rec = new(RapCfgVm.RawBytes, 0, RapCfgVm.RawBytes.Length);
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
