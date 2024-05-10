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
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Sessions
{
    public class SessionManager
    {
        private static readonly SessionManager current = new();
        public static SessionManager Current => current;

        public Dictionary<string, Session> Sessions { get; private set; } = new();

        private Container Container { get; set; } = new();

        public Dictionary<string, IDriver> Drivers { get; private set; } = new();
        public Dictionary<string, IDriverCommStream> Streams { get; private set; } = new();
        public Dictionary<string, ILogger> Loggers { get; private set; } = new();


        public SessionManager()
        {
        }

        public void Register<TService>(Type implType, string serviceKey, bool isSingleton)
        {
            Container.Register(
                serviceType: typeof(TService),
                implementationType: implType,
                serviceKey: serviceKey);
        }
        private TService CreateService<TService>(string serviceKey, string instanceKey)
        {
            var instance = Container.Resolve<TService>(serviceKey: serviceKey);

            var t = typeof(TService);
            switch (t.Name)
            {
                case nameof(IDriver):
                    Drivers.Add(instanceKey, (IDriver)instance!);
                    break;
                case nameof(IDriverCommStream):
                    Streams.Add(instanceKey, (IDriverCommStream)instance!);
                    break;
                case nameof(ILogger):
                    Loggers.Add(instanceKey, (ILogger)instance!);
                    break;
                default:
                    throw new InvalidOperationException($"Create {t.Name} error");
            }

            return instance;
        }

        public TService GetService<TService>(string serviceKey, string instanceKey)
        {
            var t = typeof(TService);
            switch (t.Name)
            {
                case nameof(IDriver):
                    {
                        if (Drivers.TryGetValue(instanceKey, out var instance))
                            return (TService)instance;
                        else
                            return CreateService<TService>(serviceKey!, instanceKey);
                    }
                case nameof(IDriverCommStream):
                    {
                        if (Streams.TryGetValue(instanceKey, out var instance))
                            return (TService)instance;
                        else
                            return CreateService<TService>(serviceKey!, instanceKey);
                    }
                case nameof(ILogger):
                    {
                        if (Loggers.TryGetValue(instanceKey, out var instance))
                            return (TService)instance;
                        else
                            return CreateService<TService>(serviceKey!, instanceKey);
                    }
                default:
                    throw new InvalidOperationException($"{t.Name}");
            }

        }

        private string[] ParseAddr(string addr)
        {
            string[] devInfo = addr.ToUpper().Split(":/@".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //foreach(string dev in devInfo)
            //    Console.Write(dev + "\t");
            //Console.WriteLine();
            return devInfo;
        }

        public Session CreateSession(string id, string streamAddr, string loggerAddr, string serviceAddr)
        {
            string[] streamAddrInfo = ParseAddr(streamAddr);
            string[] loggerAddrInfo = ParseAddr(loggerAddr);
            string[] serviceAddrInfo = ParseAddr(serviceAddr);

            var stream = GetService<IDriverCommStream>(streamAddrInfo[0], streamAddrInfo[1]);
            var logger = GetService<ILogger>(loggerAddrInfo[0], loggerAddrInfo[1]);
            //var service = GetService<IService>(serviceKey);

            Session s = new(stream, logger);
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
