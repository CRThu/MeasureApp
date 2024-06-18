using CarrotCommFramework.Drivers;
using CarrotCommFramework.Factory;
using CarrotCommFramework.Loggers;
using CarrotCommFramework.Sessions;
using CarrotCommFramework.Util;
using CarrotProtocolCommDemo.Logger;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DryIoc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CarrotProtocolCommDemo.ViewModel
{
    public partial class SessionConfigViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string deviceConfigText = "SESSION1+COM://COM250";

        [ObservableProperty]
        private string fastConfigExtraCommandText = "";

        [ObservableProperty]
        private DeviceInfo[] listedDevices = [];

        [ObservableProperty]
        private DeviceInfo? fastConfigSelectedDevice;

        [ObservableProperty]
        private Logger.AppLogger? appLogger;

        [ObservableProperty]
        private List<string> fastConfigProtocols = ["RAPV1", "CDPV1"];

        [ObservableProperty]
        private string fastConfigSelectedProtocol = "CDPV1";


        protected override void OnActivated()
        {
            WeakReferenceMessenger.Default.Register<SessionConfigViewModel, AppLogger>(this, (r, m) => r.AppLogger = m);
        }


        private void CreateSessionImpl(string cmd)
        {
            SessionConfig config = new(SessionConfig.Default)
            {
                //PresetProtocolCommands = ["RAPV1://RAPV1"],
                //PresetProtocolCommands = ["CDPV1://CDPV1"],
                PresetProtocolCommands = [$"{FastConfigSelectedProtocol}://{FastConfigSelectedProtocol}"],
                PresetLoggerCommands = ["DL://DL1"]
            };

            try
            {
                var SessionInstance = SessionFactory.Current.CreateSession(cmd, config);
                SessionInstance.Open();
                AppLogger!.Log("SessionInstance OPEN");

                // 跨vm传输实例
                WeakReferenceMessenger.Default.Send(SessionInstance);
                AppLogger!.Log($"WeakReferenceMessenger SessionInstance SEND");

            }
            catch (Exception ex)
            {
                AppLogger!.Log(ex.ToString());
                MessageBox.Show(ex.ToString());
            }

        }


        [RelayCommand]
        public void ConfigSession()
        {
            CreateSessionImpl(DeviceConfigText);
        }

        [RelayCommand]
        public void FastConfigSession()
        {
            CreateSessionImpl("~+"
                + FastConfigSelectedDevice!.ToAddr()
                + (string.IsNullOrEmpty(FastConfigExtraCommandText) ? "" : "@")
                + FastConfigExtraCommandText);
        }


        [RelayCommand]
        public void SearchDevice()
        {
            // 查找现有设备
            var deviceInfos = DriverFactory.Current.FindDevices();
            ListedDevices = deviceInfos;
            foreach (var deviceInfo in deviceInfos)
            {
                AppLogger!.Log($"{deviceInfo}");
            }
        }

        partial void OnListedDevicesChanged(DeviceInfo[] value)
        {
            FastConfigSelectedDevice = ListedDevices.FirstOrDefault();
            AppLogger!.Log($"FastConfigSelectedDevice Updated");
        }
    }
}