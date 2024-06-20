using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.IO.Ports;
using System.Diagnostics;
using System.Windows.Data;
using CarrotProtocolCommDemo.Logger;
using System.IO;
using CarrotCommFramework.Drivers;
using CarrotCommFramework.Sessions;
using CarrotCommFramework.Factory;
using CarrotCommFramework.Loggers;
using CarrotCommFramework.Util;
using CommunityToolkit.Mvvm.Messaging;
using System.Runtime;

namespace CarrotProtocolCommDemo.ViewModel
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private Logger.AppLogger appLogger = new();

        protected override void OnActivated()
        {
            ProductProvider.Current.Register<ILogger, DataLogger>("DL");
        }

        [RelayCommand]
        private void WindowLoaded()
        {
            // 跨vm传输实例
            WeakReferenceMessenger.Default.Send(AppLogger);

            AppLogger!.Log($"WINDOW LOADED");

            // GC SETTINGS
            var gcStat = GCSettings.IsServerGC ? "Server" : "Workstation";
            AppLogger!.Log($"Current GC Mode: {gcStat}.");
            //GCSettings.LatencyMode = GCLatencyMode.LowLatency;
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
            var gcLatency = GCSettings.LatencyMode;
            AppLogger!.Log($"Current GC LatencyMode: {gcLatency}.");
        }

        [RelayCommand]
        private void WindowClosed()
        {
            AppLogger!.Log($"WINDOW CLOSED");
        }
    }
}