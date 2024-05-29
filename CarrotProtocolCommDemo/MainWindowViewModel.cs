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

            SessionInstance = SessionFactory.Current.CreateSession(DeviceConfigText, SessionConfig.Default);
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
