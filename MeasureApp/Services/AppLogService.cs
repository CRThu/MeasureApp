using CarrotLink.Core.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

    public partial class AppLogService : ObservableObject, IRuntimeLogger
    {
        [ObservableProperty]
        private ObservableCollection<AppLogEntry> logs;

        public AppLogService()
        {
            Logs = new ObservableCollection<AppLogEntry>();
            Console.SetOut(new AppLogTextWriter(this));
        }

        public void Dispose()
        {
        }

        public void HandleRuntime(string message, LogLevel level = LogLevel.Info, Exception ex = null)
        {
            LogLevel internalLevel = (LogLevel)level;

            if (ex != null)
            {
                Log(ex.ToString(), internalLevel);
            }
            else
            {
                Log(message, internalLevel);
            }
        }

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

    public class AppLogTextWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.Default;

        private AppLogService _logService;

        public AppLogTextWriter(AppLogService logService)
        {
            _logService = logService;
        }

        public override void Write(string value)
        {
            _logService.Log(value, LogLevel.Info);
        }

        public override void WriteLine(string value)
        {
            _logService.Log(value, LogLevel.Info);
        }
    }
}
