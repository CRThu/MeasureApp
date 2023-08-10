using CarrotProtocolLib.Protocol;
using CarrotProtocolLib.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Logger
{
    public class ProtocolLog : ILoggerRecord
    {
        public DateTime Time { get; set; }
        public TxRx TxRx { get; set; }
        public IProtocolFrame Protocol { get; set; }

        public override string ToString()
        {
            return $"{{ Time: {Time}, " +
                $"TxRx: {TxRx}, " +
                $"Protocol: {Protocol.PayloadDisplay.Replace("\r\n", "\\r\\n")} }}";
        }
    }

    public enum TxRx
    {
        Tx,
        Rx
    }

    public enum LoggerUpdateEvent
    {
        AddEvent
    }

    public class ProtocolLogger : ILogger
    {
        public List<ProtocolLog> ProtocolList { get; set; }

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

        public void AddRx(IProtocolFrame protocol)
        {
            ProtocolLog protocolLog = new()
            {
                Protocol = protocol,
                TxRx = TxRx.Rx,
                Time = DateTime.Now
            };
            ProtocolList.Add(protocolLog);
            LoggerUpdate?.Invoke(protocolLog, LoggerUpdateEvent.AddEvent);
        }

        public void AddTx(IProtocolFrame protocol)
        {
            ProtocolLog protocolLog = new()
            {
                Protocol = protocol,
                TxRx = TxRx.Tx,
                Time = DateTime.Now
            };
            ProtocolList.Add(protocolLog);
            LoggerUpdate?.Invoke(protocolLog, LoggerUpdateEvent.AddEvent);
        }
    }
}
