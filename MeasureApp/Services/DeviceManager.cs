using CarrotLink.Core.Devices;
using CarrotLink.Core.Devices.Configuration;
using CarrotLink.Core.Discovery;
using CarrotLink.Core.Discovery.Models;
using CarrotLink.Core.Logging;
using CarrotLink.Core.Protocols;
using CarrotLink.Core.Session;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MeasureApp.Services
{
    public partial class ConnectionInfo : ObservableObject
    {
        [ObservableProperty]
        DriverType driver;

        [ObservableProperty]
        InterfaceType intf;

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
        public string errorDesc;
    }

    public partial class DeviceManager : ObservableObject, IDisposable
    {
        private readonly ConcurrentDictionary<string, DeviceSession> _sessions = new ConcurrentDictionary<string, DeviceSession>();

        [ObservableProperty]
        private ObservableCollection<ConnectionInfo> connections = new ObservableCollection<ConnectionInfo>();

        private readonly System.Threading.Timer _backgroundTimer;
        private readonly object _updateLock = new object();
        private bool _disposed;
        private readonly IRuntimeLogger _appLogger;
        private readonly IEnumerable<IPacketLogger> _packetLoggers;

        public DeviceSession this[string key] => _sessions[key];

        public DeviceManager(IRuntimeLogger appLogger, IEnumerable<IPacketLogger> packetLoggers)
        {
            _appLogger = appLogger;
            _packetLoggers = packetLoggers;
            _backgroundTimer = new System.Threading.Timer(
                callback: _ => UpdateConnectionStats(),
                state: null,
                dueTime: 100,
                period: 100);
        }

        private void UpdateConnectionStats()
        {
            if (_disposed)
                return;

            lock (_updateLock)
            {
                Application.Current?.Dispatcher.BeginInvoke(() =>
                {
                    foreach (var connection in Connections)
                    {
                        if (_sessions.TryGetValue(connection.Name, out var session))
                        {
                            try
                            {
                                connection.BytesSent = session.TotalWriteBytes;
                                connection.BytesReceived = session.TotalReadBytes;
                                connection.HasError = !session.IsAutoPollingTaskRunning;
                                connection.ErrorDesc = session.ErrorDesc;
                            }
                            catch
                            {
                                // TODO
                            }
                        }
                        else
                        {
                            // TODO
                        }
                    }
                }, DispatcherPriority.Background);
            }
        }

        public void Connect(
            DeviceInfo deviceInfo,
            DeviceConfigurationBase deviceConfig,
            ProtocolType protocolType,
            ProtocolConfigBase protocolConfig,
            bool isAutoPollingEnabled,
            string jsonConfig)
        {
            var dev = DeviceFactory.Create(deviceInfo.Interface, deviceConfig);
            dev.Connect();

            var protocol = ProtocolFactory.Create(protocolType, protocolConfig);

            var session = DeviceSession.Create()
                .WithDevice(dev)
                .WithProtocol(protocol)
                .WithLoggers(_packetLoggers)
                .WithRuntimeLogger(_appLogger)
                .WithPolling(isAutoPollingEnabled)
                .Build();

            var connectionInfo = new ConnectionInfo()
            {
                Driver = deviceInfo.Driver,
                Intf = deviceInfo.Interface,
                Name = deviceInfo.Name,
                Protocol = protocolType,
                Config = jsonConfig,
                BytesReceived = 0,
                BytesSent = 0,
                HasError = false,
                ErrorDesc = ""
            };

            if (_sessions.TryAdd(deviceInfo.Name, session))
            {
                Application.Current.Dispatcher.Invoke(() => Connections.Add(connectionInfo));
            }
            else
            {
                session.Dispose();
                throw new InvalidOperationException($"A device with name '{deviceInfo.Name}' is already connected.");
            }
        }

        public void Disconnect(string deviceName)
        {
            if (_sessions.TryRemove(deviceName, out var session))
            {
                try
                {
                    session.Device.Disconnect();
                }
                catch
                {
                    // Ignore disconnect errors during removal
                }
                finally
                {
                    session.Dispose();
                }

                var itemToRemove = Connections.FirstOrDefault(i => i.Name == deviceName);
                if (itemToRemove != null)
                {
                    Application.Current.Dispatcher.Invoke(() => Connections.Remove(itemToRemove));
                }
            }
            else
            {
                 // Not found or already removed
            }
        }



        public DeviceInfo[] DiscoverDevices()
        {
            var factory = new DeviceSearcherFactory();
            var service = new DeviceDiscoveryService(factory);
            var allDevices = service.DiscoverAll();

            return allDevices
                .OrderBy(d => d.Interface)
                .ThenBy(d => d.Name)
                .ToArray();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // 释放托管资源
                _backgroundTimer.Dispose();

                foreach (var service in _sessions.Values)
                {
                    try
                    {
                        service.Dispose();
                    }
                    catch
                    {

                    }
                }
            }
            // 释放非托管资源

            _disposed = true;
        }

        ~DeviceManager() => Dispose(false);
    }
}