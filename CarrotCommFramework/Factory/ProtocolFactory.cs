using CarrotCommFramework.Drivers;
using CarrotCommFramework.Loggers;
using CarrotCommFramework.Protocols;
using CarrotCommFramework.Sessions;
using CarrotCommFramework.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Factory
{
    public class ProtocolFactory
    {
        private static readonly ProtocolFactory current = new();
        public static ProtocolFactory Current => current;

        /// <summary>
        /// Protocols存储 Key:InstanceKey Value:IProtocol 接口派生类
        /// </summary>
        public Dictionary<string, IProtocol> Protocols { get; private set; } = new();

        public ProtocolFactory()
        {
            Register();
        }

        private static void Register()
        {
            ProductProvider.Current.Register<IProtocol, RawAsciiProtocol>("RAPV1");
            //ProductProvider.Current.Register<IProtocol, CarrotDataProtocol>("CDPV1");
        }

        public static IProtocol Create(string serviceKey, string instanceKey)
        {
            try
            {
                var x = ProductProvider.Current.Resolve<IProtocol>(serviceKey);
                x.Name = instanceKey;
                return x;
            }
            catch (Exception _)
            {
                throw new NotImplementedException($"No Protocol {serviceKey} :: {instanceKey}.");
            }
        }

        //public IProtocol Create(string serviceKey, string instanceKey)
        //{
        //    serviceKey = serviceKey.ToUpper();
        //    if (serviceKey == "RAPV1")
        //        return new RawAsciiProtocol() { Name = instanceKey };
        //    else if (serviceKey == "CDPV1")
        //        return new CarrotDataProtocol() { Name = instanceKey };
        //    else
        //        throw new NotImplementedException($"No Protocol {serviceKey}.");
        //}

        public IProtocol Get(string serviceKey, string instanceKey, IDictionary<string, string> @params = default!)
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
