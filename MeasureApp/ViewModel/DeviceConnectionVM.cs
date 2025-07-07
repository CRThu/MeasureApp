using CarrotLink.Core.Devices.Configuration;
using CarrotLink.Core.Discovery;
using CarrotLink.Core.Discovery.Models;
using CarrotLink.Core.Protocols.Impl;
using CarrotLink.Core.Protocols.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    public enum ConnectionType
    {
        Null,
        Serial,
        Usb,
        Gpib
    }

    public partial class DeviceConnectionInfo : ObservableObject
    {
        [ObservableProperty]
        ConnectionType type;

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
        private ConnectionType currentConnectionType = ConnectionType.Serial;

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



        [RelayCommand]
        private void DeviceDiscoveryRefresh()
        {
            var factory = new DeviceSearcherFactory();
            var service = new DeviceDiscoveryService(factory);
            var allDevices = service.DiscoverAll();

            AvailableDevices = allDevices.ToArray();
            SelectedDevice = AvailableDevices.FirstOrDefault();
            IsSelectedDeviceConnected = false; //TODO
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
            DevicesConnectionInfo.Add(new DeviceConnectionInfo()
            {
                Type = ConnectionType.Serial,
                Name = "COM999",
                Protocol = ProtocolType.CarrotAsciiProtocol,
                Config = "{}",
                BytesSent = 0,
                BytesReceived = 0,
                HasError = false,
                ErrorDescription = "<NULL>"
            });
        }
    }
}
