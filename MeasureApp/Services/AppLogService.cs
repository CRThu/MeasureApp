using CarrotLink.Core.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using MeasureApp.Model.Log;
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

            Application.Current?.Dispatcher?.BeginInvoke(() => Logs.Add(logEntry), DispatcherPriority.Background);
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
