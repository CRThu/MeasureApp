using CarrotProtocolLib.Device;
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

namespace IOStreamDemo.Sessions
{
    public class SessionManager
    {
        private static readonly SessionManager current = new();
        public static SessionManager Current => current;

        public Container Container { get; private set; } = new();

        public Dictionary<string, IDriver> Drivers { get; private set; } = new();
        public Dictionary<string, IDriverCommStream> Streams { get; private set; } = new();
        public Dictionary<string, ILogger> Loggers { get; private set; } = new();
        //public Dictionary<string, IService> Services { get; private set; } = new();

        public Dictionary<string, Session> Sessions { get; private set; } = new();


        public SessionManager()
        {
        }

        public void Register<TService>(Type implType, string serviceKey, bool isSingleton)
        {
            Container.Register(
                serviceType: typeof(TService),
                implementationType: implType,
                serviceKey: serviceKey,
                reuse: isSingleton ? Reuse.Singleton : null);
        }

        public Session Create(string id, string streamKey, string loggerKey, string serviceKey)
        {
            var streamInstance = Container.Resolve<IDriverCommStream>(serviceKey: streamKey);
            var loggerInstance = Container.Resolve<ILogger>(serviceKey: loggerKey);
            //var serviceInstance = Container.Resolve<IService>(serviceKey: serviceKey);

            Session s = new(streamInstance, loggerInstance);
            Sessions.Add(id, s);
            return s;
        }

        public bool TryGet(string id, out Session? session)
        {
            return Sessions.TryGetValue(id, out session);
        }

        public static DeviceInfo[] FindDevices(SessionManager current)
            => DriverManager.FindDevices(current);
    }
}
