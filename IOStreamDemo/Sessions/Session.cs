using IOStreamDemo.Loggers;
using IOStreamDemo.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Sessions
{
    public class Session
    {
        public IDriverCommStream Stream { get; set; }
        public ILogger Logger { get; set; }

        public Session(IDriverCommStream stream, ILogger logger)
        {
            Stream = stream;
            Logger = logger;
        }
    }
}
