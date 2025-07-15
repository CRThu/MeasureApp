using CarrotLink.Core.Devices;
using CarrotLink.Core.Discovery.Models;
using CarrotLink.Core.Protocols;
using CarrotLink.Core.Services;
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
        public string errorDescription;

        public string InternalKey => GetInternalKey(Driver, Intf, Name);

        public static string GetInternalKey(DriverType driver, InterfaceType intf, string name)
        {
            return $"{driver}::{intf}::{name}";
        }
    }

    public partial class DeviceManager : ObservableObject, IDisposable
    {
        public readonly ConcurrentDictionary<string, DeviceSession> _services = new ConcurrentDictionary<string, DeviceSession>();

        [ObservableProperty]
        private ObservableCollection<ConnectionInfo> info = new ObservableCollection<ConnectionInfo>();

        private readonly System.Threading.Timer _backgroundTimer;
        private readonly object _updateLock = new object();
        private bool _disposed;

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
                        if (_services.TryGetValue(infoItem.InternalKey, out var service))
                        {
                            try
                            {
                                infoItem.BytesSent = service.TotalWriteBytes;
                                infoItem.BytesReceived = service.TotalReadBytes;

                                // TODO
                                infoItem.HasError = false;
                                infoItem.ErrorDescription = "<NULL>";
                            }
                            catch (Exception ex)
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

        public void AddService(DriverType driver, InterfaceType intf, string name, ProtocolType protocol, string config, DeviceSession service)
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
                ErrorDescription = ""
            };

            if (_services.TryAdd(info.InternalKey, service))
            {
                //Application.Current.Dispatcher.BeginInvoke(() => Info.Add(info));
                Application.Current.Dispatcher.Invoke(() => Info.Add(info));
            }
            else
            {
                // TODO
            }
        }

        public void RemoveService(string key)
        {
            if (_services.TryRemove(key, out var info))
            {
                var itemToRemove = Info.FirstOrDefault(i => i.InternalKey == key);
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

                foreach (var service in _services.Values)
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