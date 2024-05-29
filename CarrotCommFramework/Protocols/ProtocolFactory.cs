using CarrotCommFramework.Drivers;
using CarrotCommFramework.Loggers;
using CarrotCommFramework.Sessions;
using CarrotCommFramework.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Protocols
{
    public class ProtocolFactory
    {
        private static readonly ProtocolFactory current = new();
        public static ProtocolFactory Current => current;

        /// <summary>
        /// Protocols存储 Key:InstanceKey Value:IProtocol 接口派生类
        /// </summary>
        public Dictionary<string, IProtocol> Protocols { get; private set; } = new();

        public IProtocol Create(string serviceKey, string instanceKey)
        {
            serviceKey = serviceKey.ToUpper();
            if (serviceKey == "RAPV1")
                return new RawAsciiProtocol(instanceKey);
            else if (serviceKey == "CDPV1")
                return new CarrotDataProtocol(instanceKey);
            else
                throw new NotImplementedException($"No Protocol {serviceKey}.");
        }

        public IProtocol Get(string serviceKey, string instanceKey, string[] @params = default!)
        {
            if (Protocols.TryGetValue(instanceKey, out var instance))
                return instance;
            else
            {
                var service = Create(serviceKey!, instanceKey);
                //service.Config(@params);
                Protocols.Add(instanceKey, service);
                return service;
            }
        }
    }
}
