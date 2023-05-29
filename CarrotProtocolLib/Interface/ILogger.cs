using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrotProtocolLib.Driver;
using CarrotProtocolLib.Impl;

namespace CarrotProtocolLib.Interface
{
    public interface ILogger
    {
        public delegate void LoggerUpdateHandler(ILoggerRecord log, LoggerUpdateEvent e);
        public event LoggerUpdateHandler LoggerUpdate;

        public void AddRx(IProtocolLog protocol);
        public void AddTx(IProtocolLog protocol);
    }

    public interface ILoggerRecord
    {

    }
}
