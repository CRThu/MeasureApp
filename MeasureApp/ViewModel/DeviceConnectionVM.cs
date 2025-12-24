using CarrotLink.Core.Devices;
using CarrotLink.Core.Devices.Configuration;
using CarrotLink.Core.Devices.Impl;
using CarrotLink.Core.Devices.Interfaces;
using CarrotLink.Core.Discovery;
using CarrotLink.Core.Discovery.Models;
using CarrotLink.Core.Logging;
using CarrotLink.Core.Protocols;
using CarrotLink.Core.Protocols.Configuration;
using CarrotLink.Core.Protocols.Impl;
using CarrotLink.Core.Protocols.Models;
using CarrotLink.Core.Session;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private readonly DeviceManager _deviceManager;
        private readonly ConfigManager _configManager;
        private readonly IRuntimeLogger _appLogger;
        private readonly IEnumerable<IPacketLogger> _packetLoggers;
        public DeviceManager DeviceManager => _deviceManager;
        public ConfigManager ConfigManager => _configManager;
        public IRuntimeLogger AppLogger => _appLogger;
        public IEnumerable<IPacketLogger> PacketLoggers => _packetLoggers;

        [ObservableProperty]
        private DeviceInfo[] availableDevices = Array.Empty<DeviceInfo>();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsSelectedDeviceConnected))]
        [NotifyPropertyChangedFor(nameof(CurrentConnectionType))]
        private DeviceInfo selectedDevice = default;

        public bool IsSelectedDeviceConnected =>
            DeviceManager.Info.Any(dev => dev.Name == SelectedDevice.Name);

        [ObservableProperty]
        private ProtocolType[] availableProtocols = Enum.GetValues<ProtocolType>();

        [ObservableProperty]
        private ProtocolType selectedProtocol = ProtocolType.CarrotAscii;

        [ObservableProperty]
        private bool isAutoPollingEnabled = true;

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

        public DeviceConnectionVM(DeviceManager deviceManager, ConfigManager configManager, IRuntimeLogger appLogger, IEnumerable<IPacketLogger> packetLoggers)
        {
            Title = "设备连接";
            ContentId = "DeviceConnection";
            _deviceManager = deviceManager;
            _configManager = configManager;
            _appLogger = appLogger;
            _packetLoggers = packetLoggers;

            DeviceManager.Info.CollectionChanged += (s, e) => OnPropertyChanged(nameof(IsSelectedDeviceConnected));
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
                AppLogger.HandleRuntime(ex.Message, LogLevel.Error);
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
                AppLogger.HandleRuntime(ex.Message, LogLevel.Error);
            }
        }


        private void ConnectDevice()
        {
            IDevice dev = null;
            DeviceConfigurationBase config;
            switch (CurrentConnectionType)
            {
                case InterfaceType.Serial:
                    config = new SerialConfiguration
                    {
                        DeviceId = $"{SelectedDevice.Interface} | {SelectedDevice.Name}",
                        PortName = SelectedDevice.Name,
                        BaudRate = SelectedSerialPortBaudRate,
                    };
                    dev = DeviceFactory.Create(InterfaceType.Serial, config);
                    break;
                case InterfaceType.NiVisa:
                    config = new NiVisaConfiguration
                    {
                        DeviceId = $"{SelectedDevice.Interface} | {SelectedDevice.Name}",
                        ResourceString = SelectedDevice.Name,
                    };
                    dev = DeviceFactory.Create(InterfaceType.NiVisa, config);
                    break;
                case InterfaceType.Ftdi:
                    config = new FtdiConfiguration
                    {
                        DeviceId = $"{SelectedDevice.Interface} | {SelectedDevice.Name}",
                        SerialNumber = SelectedDevice.Name,
                        Mode = FtdiCommMode.AsyncFifo,
                        Model = FtdiModel.Ft2232h,
                    };
                    dev = DeviceFactory.Create(InterfaceType.Ftdi, config);
                    break;
                default:
                    AppLogger.HandleRuntime($"Unsupported device type: {CurrentConnectionType}", LogLevel.Error);
                    return;
                    //throw new NotSupportedException($"Unsupported device type: {CurrentConnectionType}");
            }
            dev.Connect();

            // todo protocol config
            ProtocolConfigBase pcfg = null;
            switch (SelectedProtocol)
            {
                case ProtocolType.CarrotAscii:
                    pcfg = ConfigManager.CarrotLinkConfig.CarrotAsciiProtocolConfiguration;
                    break;
            }

            IProtocol protocol = ProtocolFactory.Create(SelectedProtocol, pcfg);

            var session = DeviceSession.Create()
                .WithDevice(dev)
                .WithProtocol(protocol)
                .WithLoggers(PacketLoggers)
                .WithRuntimeLogger(AppLogger)
                .WithPolling(IsAutoPollingEnabled)
                .Build();

            DeviceManager.AddService(
                SelectedDevice.Driver,
                SelectedDevice.Interface,
                SelectedDevice.Name,
                SelectedProtocol,
                DeviceJsonConfiguration,
                session);
        }

        private void DisonnectDevice()
        {
            string key = SelectedDevice.Name;
            DeviceManager[key].Device.Disconnect();
            DeviceManager[key].Device.Dispose();
            DeviceManager.RemoveService(key);
        }
    }
}
