using CarrotLink.Core.Devices.Configuration;
using CarrotLink.Core.Devices.Impl;
using CarrotLink.Core.Devices.Interfaces;
using CarrotLink.Core.Discovery;
using CarrotLink.Core.Discovery.Models;
using CarrotLink.Core.Protocols.Impl;
using CarrotLink.Core.Protocols.Models;
using CarrotLink.Core.Services;
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

namespace MeasureApp.ViewModel
{
    public enum ProtocolType
    {
        CarrotAsciiProtocol,
        CarrotBinaryProtocol
    }

    public partial class DeviceConnectionInfo : ObservableObject
    {
        [ObservableProperty]
        DeviceType type;

        [ObservableProperty]
        public string name;

        [ObservableProperty]
        ProtocolType protocol;

        [ObservableProperty]
        public string config;

        [ObservableProperty]
        public long bytesReceived;

        [ObservableProperty]
        public long bytesSent;

        [ObservableProperty]
        public bool hasError;

        [ObservableProperty]
        public string errorDescription;
    }

    public partial class DeviceConnectionVM : BaseVM
    {
        private readonly AppContextManager _context;

        [ObservableProperty]
        private DeviceInfo[] availableDevices = Array.Empty<DeviceInfo>();

        [ObservableProperty]
        private DeviceInfo selectedDevice = default;

        [ObservableProperty]
        private bool isSelectedDeviceConnected = false;

        [ObservableProperty]
        private ProtocolType[] availableProtocols = Enum.GetValues<ProtocolType>();

        [ObservableProperty]
        private ProtocolType selectedProtocol = ProtocolType.CarrotAsciiProtocol;

        [ObservableProperty]
        private DeviceType currentConnectionType = DeviceType.Serial;

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


        [ObservableProperty]
        private ObservableCollection<DeviceConnectionInfo> devicesConnectionInfo = new ObservableCollection<DeviceConnectionInfo>();

        public DeviceConnectionVM(AppContextManager context)
        {
            _context = context;
        }

        [RelayCommand]
        private void DeviceDiscoveryRefresh()
        {
            var factory = new DeviceSearcherFactory();
            var service = new DeviceDiscoveryService(factory);
            var allDevices = service.DiscoverAll();

            AvailableDevices = allDevices
                .OrderBy(d => d.Type)
                .ThenBy(d => d.Name)
                .ToArray();
            SelectedDevice = AvailableDevices.FirstOrDefault();

        }

        partial void OnSelectedDeviceChanged(DeviceInfo value)
        {
            if (value != null)
            {
                CurrentConnectionType = value.Type;
                UpdateUIConfigToJson();
                IsSelectedDeviceConnected = DevicesConnectionInfo.Any(
                    dev => dev.Name == SelectedDevice.Name
                    && dev.Type == SelectedDevice.Type);
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
        private void ConnectDevice()
        {
            switch (CurrentConnectionType)
            {
                case DeviceType.Serial:
                    ConnectSerialDevice();
                    break;
                case DeviceType.Ftdi:
                    //var config = new FtdiConfiguration
                    //{
                    //    DeviceId = "ftdi-1",
                    //    SerialNumber = "FTA8EKKFA",
                    //    Mode = FtdiCommMode.AsyncFifo,
                    //    Model = FtdiModel.Ft2232h,
                    //};
                    //context.Device = new FtdiDevice(config);
                    throw new NotImplementedException();
                    break;
                case DeviceType.NiVisa:
                    throw new NotImplementedException();
                    break;
                default:
                    _context.Logger.Log($"Unsupported device type: {CurrentConnectionType}", LogLevel.Error);
                    break;
                    //throw new NotSupportedException($"Unsupported device type: {CurrentConnectionType}");
            }
        }

        private void ConnectSerialDevice()
        {
            var config = new SerialConfiguration
            {
                DeviceId = $"{SelectedDevice.Type} | {SelectedDevice.Name}",
                PortName = SelectedDevice.Name,
                BaudRate = SelectedSerialPortBaudRate,
            };
            IDevice ser = new SerialDevice(config);

            IProtocol protocol;
            switch (SelectedProtocol)
            {
                case ProtocolType.CarrotAsciiProtocol:
                    protocol = new CarrotAsciiProtocol();
                    break;
                case ProtocolType.CarrotBinaryProtocol:
                    protocol = new CarrotBinaryProtocol();
                    break;
                default:
                    _context.Logger.Log($"Unsupported protocol type: {SelectedProtocol}", LogLevel.Error);
                    return;
            }

            var service = DeviceService.Create()
                .WithDevice(ser)
                .WithProtocol(protocol)
                //.WithLoggers()
                .Build();
            //Task procTask = service.StartProcessingAsync(cts.Token);
            //Task pollTask = service.StartAutoPollingAsync(15, cts.Token);
        }
    }
}
