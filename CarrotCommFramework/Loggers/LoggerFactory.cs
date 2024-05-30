using CarrotCommFramework.Drivers;
using CarrotCommFramework.Factory;
using CarrotCommFramework.Sessions;
using CarrotCommFramework.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Loggers
{
    public class LoggerFactory
    {
        private static readonly LoggerFactory current = new();
        public static LoggerFactory Current => current;

        /// <summary>
        /// Loggers存储 Key:InstanceKey Value:ILogger 接口派生类
        /// </summary>
        public Dictionary<string, ILogger> Loggers { get; private set; } = new();

        public LoggerFactory()
        {
            Register();
        }

        private static void Register()
        {
            ProductProvider.Current.Register<ILogger, ConsoleLogger>("CONSOLE");
            ProductProvider.Current.Register<ILogger, NLogLogger>("NLOG");
        }

        public static ILogger Create(string serviceKey, string instanceKey)
        {
            try
            {
                var x = ProductProvider.Current.Resolve<ILogger>(serviceKey);
                x.Name = instanceKey;
                return x;
            }
            catch (Exception _)
            {
                throw new NotImplementedException($"No Logger {serviceKey} :: {instanceKey}.");
            }
        }

        //private ILogger Create(string serviceKey, string instanceKey)
        //{
        //    serviceKey = serviceKey.ToUpper();
        //    if (serviceKey == "CONSOLE")
        //        return new ConsoleLogger() { Name = instanceKey };
        //    else if (serviceKey == "NLOG")
        //        return new NLogLogger() { Name = instanceKey };
        //    else
        //        throw new NotImplementedException($"No Logger {serviceKey}.");
        //}

        public ILogger Get(string serviceKey, string instanceKey, string[] @params = default!)
        {
            if (Loggers.TryGetValue(instanceKey, out var instance))
                return instance;
            else
            {
                var service = Create(serviceKey!, instanceKey);
                service.Config(@params);
                Loggers.Add(instanceKey, service);
                return service;
            }
        }
    }
}
