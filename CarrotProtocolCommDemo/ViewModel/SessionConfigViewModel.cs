using CarrotCommFramework.Factory;
using CarrotCommFramework.Loggers;
using CarrotCommFramework.Sessions;
using CarrotCommFramework.Util;
using CarrotProtocolCommDemo.Logger;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolCommDemo.ViewModel
{
    public partial class SessionConfigViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string deviceConfigText = "SESSION1+COM://COM250";

        [RelayCommand]
        public void Config()
        {
            SessionConfig config = new(SessionConfig.Default)
            {
                PresetLoggerCommands = ["DL://DL1"]
            };

            var SessionInstance = SessionFactory.Current.CreateSession(DeviceConfigText, config);
            SessionInstance.Open();

            // 跨vm传输实例
            WeakReferenceMessenger.Default.Send(SessionInstance);
            Trace.WriteLine("MSG SEND");


            SessionInstance.Loggers[0].Log(null, new LogEventArgs()
            {
                Time = DateTime.Now,
                From = "WPF",
                Packet = new CarrotCommFramework.Protocols.Packet("CONFIG CLICKED\n".AsciiToBytes())
            });

        }
    }
}