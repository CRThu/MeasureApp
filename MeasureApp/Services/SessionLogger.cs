using CarrotLink.Core.Logging;
using CarrotLink.Core.Protocols.Models;
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
    public record SessionLogEntry
    {
        public DateTime TimeStamp { get; init; }
        public string Sender { get; init; }
        public string Message { get; init; }

        public override string ToString()
        {
            return $"[{TimeStamp:yyyy-MM-dd hh:mm:ss.fff}] {Sender}: {Message}";
        }
    }

    public partial class SessionLogService : ObservableObject, IPacketLogger
    {
        [ObservableProperty]
        private ObservableCollection<SessionLogEntry> logs = new ObservableCollection<SessionLogEntry>();

        public void Dispose()
        {

        }

        public void HandlePacket(IPacket packet)
        {
            SessionLogEntry logEntry = new SessionLogEntry()
            {
                TimeStamp = DateTime.Now,
                Sender = "<sender>",
                Message = packet.ToString()
            };
            // TODO 性能优化
            Application.Current.Dispatcher.BeginInvoke(() => Logs.Add(logEntry), DispatcherPriority.Background);
        }
    }
}
