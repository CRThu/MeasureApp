using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrotProtocolLib.Protocol;

namespace CarrotProtocolLib.Logger
{
    public enum TransferType
    {
        Command,
        Data
    }

    public interface ILogger
    {
        public ObservableCollection<IRecord> ProtocolList { get; }

        public delegate void RecordUpdateHandler(IRecord log);
        public event RecordUpdateHandler RecordUpdate;

        public void Add(string from, string to, IProtocolFrame frame);
    }

}
