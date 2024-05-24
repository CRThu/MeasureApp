using CarrotProtocolLib.Service;
using IOStreamDemo.Loggers;
using IOStreamDemo.Protocols;
using IOStreamDemo.Services;
using IOStreamDemo.Streams;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IService = IOStreamDemo.Services.IService;

namespace IOStreamDemo.Sessions
{
    public class Session
    {
        public string Name { get; set; }
        public List<IStream> Streams { get; set; }
        public List<ILogger> Loggers { get; set; }
        public List<IProtocol> Protocols { get; set; }
        public List<IService> Services { get; set; }


        public Session()
        {
            Streams = [];
            Loggers = [];
            Protocols = [];
            Services = [];
        }

        public void Setup()
        {
            foreach (var (stream, protocol) in Streams.Zip(Protocols))
            {
                foreach (var service in Services)
                {
                    service.Bind(stream, protocol);

                    foreach (var logger in Loggers)
                    {
                        service.Logging += logger.Log;
                    }
                }
            }
        }

        public void Open()
        {
            Streams![0].Open();
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

            Streams![0].Close();

            // Wait
            foreach (var service in Services)
            {
                while (!service.Task.IsCompleted)
                    ;
                Console.WriteLine(service.ToString() + " IS STOPPED.");
            }
        }
    }
}
