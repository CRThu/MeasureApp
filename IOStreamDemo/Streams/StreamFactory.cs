using IOStreamDemo.Drivers;
using IOStreamDemo.Protocols;
using IOStreamDemo.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Streams
{
    public class StreamFactory
    {
        private static readonly StreamFactory current = new();
        public static StreamFactory Current => current;

        public Dictionary<string, IStream> Streams { get; private set; } = new();

        public static IStream Create(string serviceKey, string instanceKey)
        {
            serviceKey = serviceKey.ToUpper();
            if (serviceKey == "COM")
                return new SerialStream(instanceKey);
            else if (serviceKey == "GPIB")
                return new VisaGpibStream(instanceKey);
            else
                throw new NotImplementedException($"No Stream {serviceKey}.");
        }

        public IStream Get(string serviceKey, string instanceKey)
        {
            if (Streams.TryGetValue(instanceKey, out var instance))
                return instance;
            else
            {
                var service = Create(serviceKey!, instanceKey);
                return service;
            }
        }
    }
}
