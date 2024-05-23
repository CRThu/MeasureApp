using CarrotProtocolLib.Device;
using IOStreamDemo.Drivers;
using IOStreamDemo.Loggers;
using IOStreamDemo.Protocols;
using IOStreamDemo.Protocols;
using IOStreamDemo.Streams;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Sessions
{
    public class SessionFactory
    {
        private static readonly SessionFactory current = new();
        public static SessionFactory Current => current;

        public Dictionary<string, Session> Sessions { get; private set; } = new();

        public SessionFactory()
        {
        }


        private string[] ParseAddr(string addr)
        {
            string[] devInfo = addr.ToUpper().Split("://", 2);
            //foreach(string dev in devInfo)
            //    Console.Write(dev + "\t");
            //Console.WriteLine();
            return devInfo;
        }

        public Session CreateSession(string id, string streamAddr, string loggerAddr, string protocolAddr)
        {
            string[] streamAddrInfo = ParseAddr(streamAddr);
            string[] loggerAddrInfo = ParseAddr(loggerAddr);
            string[] protocolAddrInfo = ParseAddr(protocolAddr);

            var stream = StreamFactory.Current.Get(streamAddrInfo[0], streamAddrInfo[1]);
            var logger = LoggerFactory.Current.Get(loggerAddrInfo[0], loggerAddrInfo[1]);
            var service = ProtocolFactory.Current.Get(protocolAddrInfo[0], protocolAddrInfo[0]);

            stream.Config(streamAddrInfo[1]);

            Session s = new(stream, logger, service);
            Sessions.Add(id, s);
            return s;
        }

        public bool TryGet(string id, out Session? session)
        {
            return Sessions.TryGetValue(id, out session);
        }
    }
}
