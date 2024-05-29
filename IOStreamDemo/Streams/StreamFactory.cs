using CarrotCommFramework.Drivers;
using CarrotCommFramework.Protocols;
using CarrotCommFramework.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Streams
{
    public class StreamFactory
    {
        private static readonly StreamFactory current = new();
        public static StreamFactory Current => current;

        public Dictionary<string, IAsyncStream> Streams { get; private set; } = new();

        public static IAsyncStream Create(string serviceKey, string instanceKey)
        {
            serviceKey = serviceKey.ToUpper();
            if (serviceKey == "COM")
                return new SerialStream(instanceKey);
            //else if (serviceKey == "GPIB")
            //    return new VisaGpibStream(instanceKey);
            else
                throw new NotImplementedException($"No Stream {serviceKey}.");
        }

        public IAsyncStream Get(string serviceKey, string instanceKey, string[] @params = default!)
        {
            if (Streams.TryGetValue(instanceKey, out var instance))
                return instance;
            else
            {
                var service = Create(serviceKey!, instanceKey);
                service.Config(@params);
                Streams.Add(instanceKey, service);
                return service;
            }
        }
    }
}
