using DryIoc;
using IOStreamDemo.Drivers;
using IOStreamDemo.Loggers;
using IOStreamDemo.Services;
using IOStreamDemo.Streams;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo
{
    public class SessionContainer
    {
        private static readonly SessionContainer current = new();
        public static SessionContainer Current => current;

        public Container Container { get; private set; } = new();

        public SessionContainer()
        {
        }

        public void RegisterService(Type serviceType, Type implType, string serviceKey, bool isSingleton = true)
        {
            Container.Register(
                serviceType: serviceType,
                implementationType: implType,
                serviceKey: serviceKey,
                reuse: isSingleton ? Reuse.Singleton : null);
        }

        public TService GetService<TService>(string serviceKey)
        {
            return Container.Resolve<TService>(serviceKey: serviceKey);
        }

        public void CreateSession(string streamKey, string loggerKey, string serviceKey)
        {
            var streamInstance = Container.Resolve<IDriverCommStream>(serviceKey: streamKey);
            var loggerInstance = Container.Resolve<ILogger>(serviceKey: loggerKey);
            //var serviceInstance = Container.Resolve<IService>(serviceKey: serviceKey);
        }
    }

}
