using CarrotLink.Core.Logging;
using CarrotLink.Core.Protocols.Models;
using CarrotLink.Core.Utility;
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
    public record CommandLogEntry
    {
        public DateTime TimeStamp { get; init; }
        public string Sender { get; init; }
        public string Message { get; init; }
        public string FriendlyMessage => Message.ToEscapeString();

        public override string ToString()
        {
            return $"[{TimeStamp:yyyy-MM-dd hh:mm:ss.fff}] {Sender}: {FriendlyMessage}";
        }
    }

    public partial class CommandLogService : ObservableObject, IPacketLogger
    {
        // TODO 性能优化

        [ObservableProperty]
        private ObservableCollection<CommandLogEntry> logs = new ObservableCollection<CommandLogEntry>();

        public void Dispose()
        {

        }

        public void HandlePacket(IPacket packet, string sender)
        {
            CommandLogEntry logEntry = new CommandLogEntry()
            {
                TimeStamp = DateTime.Now,
                Sender = sender ?? "<sender>",
                Message = packet.ToString()
            };
            // TODO 线程安全问题
            Application.Current.Dispatcher.BeginInvoke(() => Logs.Add(logEntry), DispatcherPriority.Background);
        }
    }
}
