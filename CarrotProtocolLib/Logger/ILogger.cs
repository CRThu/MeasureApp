using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrotProtocolLib.Protocol;

namespace CarrotProtocolLib.Logger
{
    public interface ILogger
    {
        public ObservableCollection<ProtocolLog> ProtocolList { get; }

        public delegate void LoggerUpdateHandler(ILoggerRecord log, LoggerUpdateEvent e);
        public event LoggerUpdateHandler LoggerUpdate;

        public void Add(string from, string to, IProtocolFrame frame);
    }

    public interface ILoggerRecord
    {

    }
}
