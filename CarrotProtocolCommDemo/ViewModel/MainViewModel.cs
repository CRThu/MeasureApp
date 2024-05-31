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
using CarrotCommFramework.Util;

namespace CarrotProtocolCommDemo.ViewModel
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string deviceConfigText = "SESSION1+COM://COM250";

        [ObservableProperty]
        private string scriptText = "SCRIPT";

        [ObservableProperty]
        private Session? sessionInstance;

        [RelayCommand]
        public void Config()
        {
            ProductProvider.Current.Register<ILogger, DataLogger>("DL");

            SessionConfig config = new(SessionConfig.Default)
            {
                PresetLoggerCommands = ["DL://DL1"]
            };

            SessionInstance = SessionFactory.Current.CreateSession(DeviceConfigText, config);
            SessionInstance.Open();

            SessionInstance.Loggers[0].Log(null, new LogEventArgs()
            {
                Packet = new CarrotCommFramework.Protocols.Packet("CONFIG CLICKED\n".AsciiToBytes())
            });

        }
        [RelayCommand]
        public void Send()
        {
            SessionInstance.Loggers[0].Log(null, new LogEventArgs()
            {
                Packet = new CarrotCommFramework.Protocols.Packet("SEND CLICKED\n".AsciiToBytes())
            });

            SessionInstance!.Write(ScriptText);

            SessionInstance.Loggers[0].Log(null, new LogEventArgs()
            {
                Packet = new CarrotCommFramework.Protocols.Packet($"SESSION WRITE:{ScriptText}\n".AsciiToBytes())
            });
        }
    }
}
