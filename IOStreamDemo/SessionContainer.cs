using CarrotProtocolLib.Service;
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

        public readonly Container container = new Container();

        private Dictionary<string, IDriverCommStream> Stream { get; set; } = new();
        private Dictionary<string, ILogger> Logger { get; set; } = new();
        private Dictionary<string, IService> Service { get; set; } = new();

        public SessionContainer()
        {
        }

        public IDriverCommStream GetStream(string name)
        {
            return Stream[name];
        }
        public ILogger GetLogger(string name)
        {
            return Logger[name];
        }
        public IService GetService(string name)
        {
            return Service[name];
        }
    }

}
