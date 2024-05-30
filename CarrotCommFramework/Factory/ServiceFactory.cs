using CarrotCommFramework.Loggers;
using CarrotCommFramework.Protocols;
using CarrotCommFramework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Factory
{
    public class ServiceFactory
    {
        private static readonly ServiceFactory current = new();
        public static ServiceFactory Current => current;

        /// <summary>
        /// Services 存储 Key:InstanceKey Value:IProtocol 接口派生类
        /// </summary>
        public Dictionary<string, ISessionServiceBase> Services { get; private set; } = new();

        public ServiceFactory()
        {
            Register();
        }

        private static void Register()
        {
            ProductProvider.Current.Register<ISessionServiceBase, DataRecvService>("RECV");
            ProductProvider.Current.Register<ISessionServiceBase, ProtocolParseService>("PARSE");
        }

        public static ISessionServiceBase Create(string serviceKey, string instanceKey)
        {
            try
            {
                var x = ProductProvider.Current.Resolve<ISessionServiceBase>(serviceKey);
                x.Name = instanceKey;
                return x;
            }
            catch (Exception _)
            {
                throw new NotImplementedException($"No Service {serviceKey} :: {instanceKey}.");
            }
        }

        //public ISessionServiceBase Create(string serviceKey, string instanceKey)
        //{
        //    serviceKey = serviceKey.ToUpper();
        //    if (serviceKey == "RECV")
        //        return new DataRecvService() { Name = instanceKey };
        //    else if (serviceKey == "PARSE")
        //        return new ProtocolParseService() { Name = instanceKey };
        //    else
        //        throw new NotImplementedException($"No Service {serviceKey}.");
        //}

        public ISessionServiceBase Get(string serviceKey, string instanceKey, string[] @params = default!)
        {
            if (Services.TryGetValue(instanceKey, out var instance))
                return instance;
            else
            {
                var service = Create(serviceKey!, instanceKey);
                //service.Config(@params);
                Services.Add(instanceKey, service);
                return service;
            }
        }
    }
}
