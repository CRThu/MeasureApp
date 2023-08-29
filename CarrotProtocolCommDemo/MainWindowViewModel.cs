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
using System.Windows.Data;
using CarrotProtocolCommDemo.Logger;
using System.IO;

namespace CarrotProtocolCommDemo
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private IDevice device;

        [ObservableProperty]
        private ProtocolLogger logger;

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
        private RegisterProtocolConfigViewModel rpCfgVm = new();

        [ObservableProperty]
        public bool isOpen;

        Random random = new Random();


        public MainWindowViewModel()
        {
            Device = new GeneralBufferedDevice("SerialPort", "CarrotDataProtocol", Logger);
            Logger = new ProtocolLogger();

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

                    Device = DeviceFactory.GetDevice("GeneralBufferedDevice", SelectedDriver, SelectedProtocolName, Logger);

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

        /// <summary>
        /// 发送协议数据
        /// </summary>
        [RelayCommand]
        private void Send()
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

        [RelayCommand]
        private void DataSourceOperation(string param)
        {
            switch (param)
            {
                case "RemoveKey":
                    if (Logger.DataLogger.Ds.CurrentKey is not null)
                        Logger.DataLogger.Ds.RemoveKey(Logger.DataLogger.Ds.CurrentKey);
                    break;
                case "RemoveValues":
                    if (Logger.DataLogger.Ds.CurrentKey is not null)
                        Logger.DataLogger.Ds.RemoveValues(Logger.DataLogger.Ds.CurrentKey);
                    break;
                case "TestValues":
                    if (Logger is not null)
                    {
                        Task task = Task.Run(() =>
                        {
                            string keyName = Guid.NewGuid().ToString()[0..6];
                            //string keyName = "111111";

                            double[] doubles = new double[1000000];
                            for (int i = 0; i < doubles.Length; i++)
                                doubles[i] = random.NextDouble();
                            Logger.DataLogger.Ds.AddValue(keyName, doubles);
                        });
                        //task.Wait();
                    }
                    break;
                case "SaveValues":
                    var data = Logger.DataLogger.Ds.DisplayData;
                    using (StreamWriter writer = new("output.values.txt"))
                    {
                        for (int i = 0; i < data.Count; i++)
                            writer.WriteLine(data[i]);
                    }
                    break;
            }
        }

        [RelayCommand]
        private void LoggerRecordOperation(string param)
        {
            switch (param)
            {
                case "ClearRecord":
                    Logger.Clear();
                    break;
                case "TestRecords":
                    if (Logger is not null)
                    {
                        Task task = Task.Run(() =>
                        {
                            RawAsciiProtocolFrame[] frames = new RawAsciiProtocolFrame[1000000];
                            for (int i = 0; i < frames.Length; i++)
                                frames[i] = new RawAsciiProtocolFrame("test");
                            Logger.Add("test", "test", frames);
                        });
                        //task.Wait();
                    }
                    break;
                case "SaveRecords":
                    var data = Logger.protocolList;
                    using (StreamWriter writer = new("output.records.txt"))
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            string hexString = BitConverter.ToString(data[i].Frame.FrameBytes).Replace("-", "");
                            writer.WriteLine(hexString);
                        }
                    }
                    break;
            }
        }
    }
}
