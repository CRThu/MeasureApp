﻿using CarrotLink.Core.Devices;
using CarrotLink.Core.Devices.Configuration;
using CarrotLink.Core.Devices.Impl;
using CarrotLink.Core.Devices.Interfaces;
using CarrotLink.Core.Discovery;
using CarrotLink.Core.Discovery.Models;
using CarrotLink.Core.Protocols;
using CarrotLink.Core.Protocols.Impl;
using CarrotLink.Core.Protocols.Models;
using CarrotLink.Core.Session;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControlzEx.Standard;
using MeasureApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeasureApp.ViewModel
{
    public partial class DeviceConnectionVM : BaseVM
    {
        private readonly AppContextManager _context;
        public AppContextManager Context => _context;

        [ObservableProperty]
        private DeviceInfo[] availableDevices = Array.Empty<DeviceInfo>();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsSelectedDeviceConnected))]
        [NotifyPropertyChangedFor(nameof(CurrentConnectionType))]
        private DeviceInfo selectedDevice = default;

        public bool IsSelectedDeviceConnected =>
            _context.Devices.Info.Any(
                dev => dev.Name == SelectedDevice.Name
                && dev.Driver == SelectedDevice.Driver
                && dev.Intf == SelectedDevice.Interface);

        [ObservableProperty]
        private ProtocolType[] availableProtocols = Enum.GetValues<ProtocolType>();

        [ObservableProperty]
        private ProtocolType selectedProtocol = ProtocolType.CarrotAscii;

        public InterfaceType CurrentConnectionType => SelectedDevice != null ? SelectedDevice.Interface : InterfaceType.Serial;

        [ObservableProperty]
        private string deviceJsonConfiguration = "";


        [ObservableProperty]
        private int[] serialPortBaudRates = new int[] { 9600, 38400, 115200, 921600, 1000000, 2000000, 6000000, 12000000 };

        [ObservableProperty]
        private int selectedSerialPortBaudRate = 115200;

        [ObservableProperty]
        private SerialParity[] serialPortParitys = Enum.GetValues<SerialParity>();

        [ObservableProperty]
        private SerialParity selectedSerialPortParity = SerialParity.None;

        [ObservableProperty]
        private int[] serialPortDataBits = new int[] { 5, 6, 7, 8 };

        [ObservableProperty]
        private int selectedSerialPortDataBit = 8;

        [ObservableProperty]
        private SerialStopBits[] serialPortStopBits = Enum.GetValues<SerialStopBits>();

        [ObservableProperty]
        private SerialStopBits selectedSerialPortStopBit = SerialStopBits.One;

        public DeviceConnectionVM(AppContextManager context)
        {
            _context = context;
            _context.Devices.Info.CollectionChanged += (s, e) => OnPropertyChanged(nameof(IsSelectedDeviceConnected));
        }

        [RelayCommand]
        private void DeviceDiscoveryRefresh()
        {
            try
            {
                var factory = new DeviceSearcherFactory();
                var service = new DeviceDiscoveryService(factory);
                var allDevices = service.DiscoverAll();

                AvailableDevices = allDevices
                    .OrderBy(d => d.Interface)
                    .ThenBy(d => d.Name)
                    .ToArray();

                if (!AvailableDevices.Contains(SelectedDevice))
                    SelectedDevice = AvailableDevices.FirstOrDefault();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                _context.AppLogger.Log(ex.Message, LogLevel.Error);
            }
        }

        partial void OnSelectedDeviceChanged(DeviceInfo value)
        {
            if (value != null)
            {
                UpdateUIConfigToJson();
            }
        }

        private void UpdateUIConfigToJson()
        {
            // TODO
            DeviceJsonConfiguration = "{}";
        }

        [RelayCommand]
        private void FormatJson()
        {

        }

        [RelayCommand]
        private void ApplyJson()
        {

        }

        [RelayCommand]
        private void ConnectOrDisconnectDevice(bool isOpen)
        {
            try
            {
                if (!isOpen)
                {
                    // Connect
                    ConnectDevice();
                }
                else
                {
                    // Disconnect
                    DisonnectDevice();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                _context.AppLogger.Log(ex.Message, LogLevel.Error);
            }
        }


        private void ConnectDevice()
        {
            IDevice dev = null;
            switch (CurrentConnectionType)
            {
                case InterfaceType.Serial:
                    var config = new SerialConfiguration
                    {
                        DeviceId = $"{SelectedDevice.Interface} | {SelectedDevice.Name}",
                        PortName = SelectedDevice.Name,
                        BaudRate = SelectedSerialPortBaudRate,
                    };
                    dev = DeviceFactory.Create(InterfaceType.Serial, config);
                    dev.Connect();
                    break;
                case InterfaceType.Ftdi:
                case InterfaceType.NiVisa:
                default:
                    _context.AppLogger.Log($"Unsupported device type: {CurrentConnectionType}", LogLevel.Error);
                    break;
                    //throw new NotSupportedException($"Unsupported device type: {CurrentConnectionType}");
            }

            dev.Connect();

            // todo protocol config
            IProtocol protocol = ProtocolFactory.Create(SelectedProtocol, null);

            var session = DeviceSession.Create()
                .WithDevice(dev)
                .WithProtocol(protocol)
                .WithLogger(Context.CommandLogger)
                .Build();

            _context.Devices.AddService(
                SelectedDevice.Driver,
                SelectedDevice.Interface,
                SelectedDevice.Name,
                SelectedProtocol,
                DeviceJsonConfiguration,
                session);
        }

        private void DisonnectDevice()
        {
            string key = ConnectionInfo.GetInternalKey(SelectedDevice.Driver, SelectedDevice.Interface, SelectedDevice.Name);
            _context.Devices[key].Device.Disconnect();
            _context.Devices[key].Device.Dispose();
            _context.Devices[key].Dispose();
            _context.Devices.RemoveService(key);
        }
    }
}
