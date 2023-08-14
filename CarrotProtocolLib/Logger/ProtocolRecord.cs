using CarrotProtocolLib.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Logger
{
    public class ProtocolRecord : IRecord
    {
        public DateTime Time { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int Stream { get; set; }
        public string ProtocolName { get; set; }
        public TransferType Type { get; set; }
        public IProtocolFrame? Frame { get; set; }

        public ProtocolRecord()
        {
            Time = DateTime.Now;
            From = string.Empty;
            To = string.Empty;
            Stream = 0;
            ProtocolName = string.Empty;
        }

        public override string ToString()
        {
            return $"{{ Time: {Time}, " +
                $"From: {From}, " +
                $"To: {To}, " +
                $"Stream: {Stream}, " +
                $"Protocol: {ProtocolName}, " +
                $"Type: {Type}, " +
                $"Frame Payload: {(Type == TransferType.Data ? "<DATA>" : Frame.PayloadDisplay)} }}";
        }
    }
}
