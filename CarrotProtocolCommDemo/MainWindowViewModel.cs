using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

namespace CarrotProtocolCommDemo
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string deviceConfigText = "SESSION1+COM://COM250";

        [ObservableProperty]
        private string scriptText = "SCRIPT";

        [ObservableProperty]
        private string loggerText = "LOGGER\n";

        public Session? SessionInstance { get; set; }

        [RelayCommand]
        public void Config()
        {
            LoggerText += "CONFIG CLICKED\n";

            ProductProvider.Current.Register<ILogger, DataLogger>("DL");

            SessionConfig config = new(SessionConfig.Default)
            {
                PresetLoggerCommands = ["DL://DL1"]
            };

            SessionInstance = SessionFactory.Current.CreateSession(DeviceConfigText, config);
            SessionInstance.Open();
        }
        [RelayCommand]
        public void Send()
        {
            LoggerText += "SEND CLICKED\n";

            SessionInstance!.Write(ScriptText);
            LoggerText += $"SESSION WRITE:{ScriptText}\n";
        }
    }
}
