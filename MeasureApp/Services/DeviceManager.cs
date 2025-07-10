using CarrotLink.Core.Discovery.Models;
using CarrotLink.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using MeasureApp.ViewModel;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.Services
{
    public partial class ConnectionInfo : ObservableObject
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

        public string InternalKey => $"{Type}::{Name}";
    }

    public partial class DeviceManager : ObservableObject
    {
        public readonly ConcurrentDictionary<string, DeviceService> _services = new ConcurrentDictionary<string, DeviceService>();

        [ObservableProperty]
        private ObservableCollection<ConnectionInfo> info = new ObservableCollection<ConnectionInfo>();

        private readonly System.Timers.Timer _uiUpdateTimer;

        public DeviceManager()
        {
            _uiUpdateTimer = new System.Timers.Timer(100);
            _uiUpdateTimer.Elapsed += (s, e) => UpdateUI();
            _uiUpdateTimer.AutoReset = true;
            _uiUpdateTimer.Start();
        }

        private void UpdateUI()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var infoItem in Info)
                {
                    if (_services.TryGetValue(infoItem.InternalKey, out var service))
                    {
                        infoItem.BytesSent = service.TotalWriteBytes;
                        infoItem.BytesReceived = service.TotalReadBytes;

                        // TODO
                        infoItem.HasError = false;
                        infoItem.ErrorDescription = "<NULL>";
                    }
                    else
                    {
                        // TODO
                    }
                }
            });
        }

        public void AddService(DeviceType type, string name, ProtocolType protocol, string config, DeviceService service)
        {
            var info = new ConnectionInfo()
            {
                Type = type,
                Name = name,
                Protocol = protocol,
                Config = config,
                BytesReceived = 0,
                BytesSent = 0,
                HasError = false,
                ErrorDescription = ""
            };

            if (_services.TryAdd(info.InternalKey,service))
            {

            }
            else
            {
                // TODO
            }
        }

        public void RemoveService(string key)
        {

        }
    }
}