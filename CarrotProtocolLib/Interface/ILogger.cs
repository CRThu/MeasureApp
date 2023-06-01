using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrotProtocolLib.Impl;
using CarrotProtocolLib.Logger;

namespace CarrotProtocolLib.Interface
{
    public interface ILogger
    {
        public delegate void LoggerUpdateHandler(ILoggerRecord log, LoggerUpdateEvent e);
        public event LoggerUpdateHandler LoggerUpdate;

        public void AddRx(IProtocolRecord protocol);
        public void AddTx(IProtocolRecord protocol);
    }

    public interface ILoggerRecord
    {

    }
}
