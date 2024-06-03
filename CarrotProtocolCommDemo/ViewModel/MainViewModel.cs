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

namespace CarrotProtocolCommDemo.ViewModel
{
    public partial class MainViewModel : ViewModelBase
    {
        [RelayCommand]
        private void WindowLoaded()
        {
            Trace.WriteLine($"WINDOW LOADED\n");
            ProductProvider.Current.Register<ILogger, DataLogger>("DL");
        }

        [RelayCommand]
        private void WindowClosed()
        {
            Trace.WriteLine($"WINDOW CLOSED\n");
        }
    }
}