using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrotProtocolLib.Protocol;
using CarrotProtocolLib.Protocol;

namespace CarrotProtocolLib.Logger
{
    public interface ILogger
    {
        public delegate void LoggerUpdateHandler(ILoggerRecord log, LoggerUpdateEvent e);
        public event LoggerUpdateHandler LoggerUpdate;

        public void AddRx(IProtocolFrame protocol);
        public void AddTx(IProtocolFrame protocol);
    }

    public interface ILoggerRecord
    {

    }
}
