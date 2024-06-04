using CarrotCommFramework.Loggers;
using CarrotCommFramework.Protocols;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolCommDemo.Logger
{
    public struct AppDebugRecord(string msg)
    {
        public DateTime Time { get; set; } = DateTime.Now;
        public string Msg { get; set; } = msg;
    }

    public partial class AppLogger : ObservableLoggerBase
    {
        [ObservableProperty]
        private ObservableCollection<AppDebugRecord> ds;

        public AppLogger() :
            base()
        {
            ds = [];
        }

        public void Log(string e)
        {
            Ds.Add(new AppDebugRecord(e));
        }
    }
}
