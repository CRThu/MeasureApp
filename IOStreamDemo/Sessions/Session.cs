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
        public string Name { get; set; }
        public List<IStream> Streams { get; set; }
        public List<ILogger> Loggers { get; set; }
        public List<IProtocol> Protocols { get; set; }
        public List<Services.IService> Services { get; set; }


        public Session()
        {
            Services = [];
            Pipe pipe = new();
            Services.Add(new DataRecvService(pipe, (IAsyncStream)Streams));
            Services.Add(new ProtocolParseService(pipe, Protocols, Loggers));
        }

        public void Open()
        {
            Streams.Open();
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

            Streams.Close();

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
