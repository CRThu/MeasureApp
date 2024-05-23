using IOStreamDemo.Drivers;
using IOStreamDemo.Sessions;
using IOStreamDemo.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Loggers
{
    public class LoggerFactory
    {
        private static readonly LoggerFactory current = new();
        public static LoggerFactory Current => current;

        /// <summary>
        /// Loggers存储 Key:InstanceKey Value:ILogger 接口派生类
        /// </summary>
        public Dictionary<string, ILogger> Loggers { get; private set; } = new();

        private ILogger Create(string serviceKey, string instanceKey)
        {
            serviceKey = serviceKey.ToUpper();
            if (serviceKey == "CONSOLE")
                return new ConsoleLogger(instanceKey);
            else if (serviceKey == "NLOG")
                return new NLogLogger(instanceKey);
            else
                throw new NotImplementedException($"No Logger {serviceKey}.");
        }

        public ILogger Get(string serviceKey, string instanceKey)
        {
            if (Loggers.TryGetValue(instanceKey, out var instance))
                return instance;
            else
            {
                var service = Create(serviceKey!, instanceKey);
                return service;
            }
        }
    }
}
