using CarrotCommFramework.Loggers;
using CarrotCommFramework.Protocols;
using CarrotCommFramework.Services;
using CarrotCommFramework.Streams;
using CarrotCommFramework.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Sessions
{
    public class Session
    {
        public string Name { get; set; }
        public List<IAsyncStream> Streams { get; set; }
        public List<ILogger> Loggers { get; set; }
        public List<IProtocol> Protocols { get; set; }
        public List<ISessionServiceBase> Services { get; set; }


        public Session()
        {
            Streams = [];
            Loggers = [];
            Protocols = [];
            Services = [];
        }

        public void Bind()
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
                service.Start();
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
                while (service.Status != ServiceStatus.Running)
                    ;
                Debug.WriteLine(service.ToString() + " IS STOPPED.");
            }
        }

        public void Write(Packet s)
        {
            foreach (var stream in Streams)
            {
                var tx = s.Bytes;
                stream.Write(tx, 0, tx.Length);

                foreach (var logger in Loggers)
                {
                    logger.Log(this, new LogEventArgs()
                    {
                        Time = DateTime.Now,
                        From = "TX",
                        Packet = s
                    });
                }
            }
        }


        public int Read(byte[] buffer, int offset, int count)
        {
            return Streams[0].Read(buffer, offset, count);
        }

        public bool TryReadLast(string filter, out Packet? packet)
        {
            int loggerNo = Convert.ToInt16(filter);
            if (Loggers.Count > loggerNo)
            {
                return Loggers[loggerNo].TryGet(-1, out packet);
            }
            else
            {
                packet = null;
                return false;
            }
        }
    }
}
