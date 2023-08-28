using CarrotProtocolLib.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Logger
{
    public enum TransferType
    {
        Command,
        Data,
        Register
    }

    public interface IRecord
    {
        public DateTime Time { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int Stream { get; set; }
        public string ProtocolName { get; set; }
        public TransferType Type { get; set; }
        public IProtocolFrame Frame { get; set; }
    }
}
