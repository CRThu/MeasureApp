using IOStreamDemo.Loggers;
using IOStreamDemo.Protocols;
using IOStreamDemo.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Services
{
    public interface IService
    {
        public Task Task { get; set; }

        public delegate void LogEventHandler(object sender, LogEventArgs e);
        public event LogEventHandler Logging;

        public void Bind(IStream stream, IProtocol protocol);

        public void Run();
        public void Stop();
    }

}
