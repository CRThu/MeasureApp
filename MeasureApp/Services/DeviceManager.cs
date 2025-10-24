using CarrotLink.Core.Devices;
using CarrotLink.Core.Discovery.Models;
using CarrotLink.Core.Protocols;
using CarrotLink.Core.Session;
using CommunityToolkit.Mvvm.ComponentModel;
using MeasureApp.ViewModel;
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
        public readonly ConcurrentDictionary<string, DeviceSession> _sessions = new ConcurrentDictionary<string, DeviceSession>();

        [ObservableProperty]
        private ObservableCollection<ConnectionInfo> info = new ObservableCollection<ConnectionInfo>();

        private readonly System.Threading.Timer _backgroundTimer;
        private readonly object _updateLock = new object();
        private bool _disposed;

        public DeviceSession this[string key] => _sessions[key];

        public DeviceManager()
        {
            _backgroundTimer = new System.Threading.Timer(
                callback: _ => UpdateUI(),
                state: null,
                dueTime: 100,
                period: 100);
        }

        private void UpdateUI()
        {
            if (_disposed)
                return;

            lock (_updateLock)
            {
                Application.Current?.Dispatcher.BeginInvoke(() =>
                {
                    foreach (var infoItem in Info)
                    {
                        if (_sessions.TryGetValue(infoItem.Name, out var session))
                        {
                            try
                            {
                                infoItem.BytesSent = session.TotalWriteBytes;
                                infoItem.BytesReceived = session.TotalReadBytes;
                                infoItem.HasError = !session.IsAutoPollingTaskRunning;
                                infoItem.ErrorDesc = session.ErrorDesc;
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

        public void AddService(DriverType driver, InterfaceType intf, string name, ProtocolType protocol, string config, DeviceSession session)
        {
            var info = new ConnectionInfo()
            {
                Driver = driver,
                Intf = intf,
                Name = name,
                Protocol = protocol,
                Config = config,
                BytesReceived = 0,
                BytesSent = 0,
                HasError = false,
                ErrorDesc = ""
            };

            if (_sessions.TryAdd(name, session))
            {
                //Application.Current.Dispatcher.BeginInvoke(() => Info.Add(info));
                Application.Current.Dispatcher.Invoke(() => Info.Add(info));
            }
            else
            {
                session.Dispose();
                throw new InvalidOperationException($"A device with name '{name}' is already connected.");
            }
        }

        public void RemoveService(string key)
        {
            if (_sessions.TryRemove(key, out var session))
            {
                session.Dispose();
                var itemToRemove = Info.FirstOrDefault(i => i.Name == key);
                if (itemToRemove != null)
                {
                    //Application.Current.Dispatcher.BeginInvoke(() => Info.Remove(itemToRemove));
                    Application.Current.Dispatcher.Invoke(() => Info.Remove(itemToRemove));
                }
            }
            else
            {
                // TODO
            }
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