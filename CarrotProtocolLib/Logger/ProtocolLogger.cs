using CarrotProtocolLib.Protocol;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Logger
{
    public class ProtocolLog : ILoggerRecord
    {
        public DateTime Time { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Protocol { get; set; }
        public TransferType Type { get; set; }
        public IProtocolFrame? Frame { get; set; }

        public ProtocolLog()
        {
            Time = DateTime.Now;
            From = string.Empty;
            To = string.Empty;
            Protocol = string.Empty;
        }

        public override string ToString()
        {
            return $"{{ Time: {Time}, " +
                $"From: {From}, " +
                $"To: {To}, " +
                $"Protocol: {Protocol}, " +
                $"Type: {Type}, " +
                $"Frame Payload: {(Type == TransferType.Data ? "<DATA>" : Frame.PayloadDisplay)} }}";
        }
    }

    public enum TransferType
    {
        Command,
        Data
    }

    public enum LoggerUpdateEvent
    {
        AddEvent
    }

    public partial class ProtocolLogger : ObservableObject, ILogger
    {
        [ObservableProperty]
        private ObservableCollection<ProtocolLog> protocolList;

        // public delegate void LoggerUpdateHandler(ILoggerRecord log, LoggerUpdateEvent e);
        public event ILogger.LoggerUpdateHandler LoggerUpdate;

        public ProtocolLogger()
        {
            ProtocolList = new();
        }

        public ProtocolLogger(ILogger.LoggerUpdateHandler loggerUpdateHandler) : this()
        {
            LoggerUpdate += loggerUpdateHandler;
        }

        public void Add(string from, string to, IProtocolFrame frame)
        {
            ProtocolLog log = new()
            {
                Time = DateTime.Now,
                From = from,
                To = to,
                Protocol = "<NULL>",
                Type = TransferType.Command,
                Frame = frame
            };
            ProtocolList.Add(log);
            LoggerUpdate?.Invoke(log, LoggerUpdateEvent.AddEvent);
        }
    }
}
