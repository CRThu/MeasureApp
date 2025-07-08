using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MeasureApp.Services
{
    public record AppLogEntry
    {
        public DateTime TimeStamp { get; init; }
        public LogLevel LogLevel { get; init; }
        public string Message { get; init; }

        public override string ToString()
        {
            return $"[{TimeStamp:yyyy-MM-dd hh:mm:ss.fff}] {LogLevel.ToString().ToUpper()}: {Message}";
        }
    }

    public partial class AppLogService : ObservableObject, ILogService
    {
        [ObservableProperty]
        private ObservableCollection<AppLogEntry> logs = new ObservableCollection<AppLogEntry>();

        public void Log(string message, LogLevel level = LogLevel.Info)
        {
            AppLogEntry logEntry = new AppLogEntry()
            {
                TimeStamp = DateTime.Now,
                LogLevel = level,
                Message = message
            };

            Application.Current.Dispatcher.BeginInvoke(() => Logs.Add(logEntry), DispatcherPriority.Background);
        }
    }
}
