using CarrotProtocolLib.Service;
using IOStreamDemo.Loggers;
using IOStreamDemo.Protocols;
using IOStreamDemo.Protocols;
using IOStreamDemo.Services;
using IOStreamDemo.Streams;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Sessions
{
    public class Session
    {
        public IDriverCommStream Stream { get; set; }
        public ILogger Logger { get; set; }
        public IProtocol Protocol { get; set; }
        public List<Services.IService> Services { get; set; }


        public Session(IDriverCommStream stream, ILogger logger, IProtocol protocol)
        {
            Stream = stream;
            Logger = logger;
            Protocol = protocol;
            Services = [];
            Pipe pipe = new();
            Services.Add(new DataRecvService(pipe, (IAsyncStream)Stream));
            Services.Add(new ProtocolParseService(pipe, Protocol));
        }

        public void Open()
        {
            Stream.Open();
            foreach (var service in Services)
            {
                service.Run();
            }
        }

        public void Close()
        {
            foreach (var service in Services)
            {
                service.Stop();
            }

            Stream.Close();

            // Wait
            foreach (var service in Services)
            {
                while (!service.Task.IsCompleted)
                    ;
                Console.WriteLine(service.ToString()+" IS STOPPED.");
            }
        }
    }
}
