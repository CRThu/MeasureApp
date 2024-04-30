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
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo
{
    public class SessionContainer
    {
        private static readonly SessionContainer current = new();
        public static SessionContainer Current => current;

        public readonly Container Container = new Container();

        public SessionContainer()
        {
        }

        public IDriverCommStream GetStream(string name)
        {
            return Container.Resolve<IDriverCommStream>(serviceKey: name);
        }

        public ILogger GetLogger(string name)
        {
            return Container.Resolve<ILogger>(serviceKey: name);
        }

        public IService GetService(string name)
        {
            return Container.Resolve<IService>(serviceKey: name);
        }

        public void Add<T>(string name)
        {
            Container.Register<T>(serviceKey: name);
        }
    }

}
