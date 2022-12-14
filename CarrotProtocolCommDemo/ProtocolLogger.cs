using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolCommDemo
{
    public class ProtocolLog
    {
        public DateTime Time { get; set; }
        public TxRx TxRx { get; set; }
        public CarrotDataProtocol Protocol { get; set; }

        public override string ToString()
        {
            return $"{{ Time: {Time}, " +
                $"TxRx: {TxRx}, " +
                $"Protocol: {Protocol} }}";
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

        public delegate void LoggerUpdateHandler(ProtocolLog log, LoggerUpdateEvent e);
        public event LoggerUpdateHandler LoggerUpdate;

        public ProtocolLogger()
        {
            ProtocolList = new();
        }

        public void AddRx(CarrotDataProtocol protocol)
        {
            ProtocolLog protocolLog = new()
            {
                Protocol = protocol,
                TxRx = TxRx.Rx,
                Time = DateTime.Now
            };
            ProtocolList.Add(protocolLog);
            LoggerUpdate.Invoke(protocolLog, LoggerUpdateEvent.AddEvent);
        }

        public void AddTx(CarrotDataProtocol protocol)
        {
            ProtocolLog protocolLog = new()
            {
                Protocol = protocol,
                TxRx = TxRx.Tx,
                Time = DateTime.Now
            };
            ProtocolList.Add(protocolLog);
            LoggerUpdate.Invoke(protocolLog, LoggerUpdateEvent.AddEvent);
        }
    }
}
