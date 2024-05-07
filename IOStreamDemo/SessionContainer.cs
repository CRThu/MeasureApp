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

        public Dictionary<string, IDriverCommStream> StreamsContainer = [];
        public Dictionary<string, ILogger> LogsContainer = [];
        //public Dictionary<string, IService> ServicesContainer = [];

        public SessionContainer()
        {
        }

        public T? GetOrCreate<T>(Type implType, string key)
        {
            if (typeof(T).Name == nameof(IDriverCommStream))
            {
                if (StreamsContainer.TryGetValue(key, out IDriverCommStream? value))
                    return (T)value;
                else
                {
                    StreamsContainer[key] = (implType.Assembly.CreateInstance(implType.FullName!) as IDriverCommStream)!;
                    return (T)StreamsContainer[key];
                }
            }
            else if (typeof(T).Name == nameof(ILogger))
            {
                if (LogsContainer.TryGetValue(key, out ILogger? value))
                    return (T)value;
                else
                {
                    LogsContainer[key] = (implType.Assembly.CreateInstance(implType.FullName!) as ILogger)!;
                    return (T)LogsContainer[key];
                }
            }
            else
                return default;
        }

        public void Delete<T>(string key)
        {
            if (typeof(T).Name == nameof(IDriverCommStream))
            {
                StreamsContainer.Remove(key);
            }
            else
                return;
        }
    }

}
