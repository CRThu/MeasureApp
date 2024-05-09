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
    public class LoggerResourceInfo(string name, Type type)
    {
        public string? Name { get; set; } = name;
        public Type? Type { get; set; } = type;
    }

    public class LoggerManager
    {
        public static LoggerResourceInfo[] RegisteredResources =
        [
            new LoggerResourceInfo("CONSOLE", typeof(ConsoleLogger)),
            new LoggerResourceInfo("NLOG", typeof(NLogLogger)),
        ];

        public static void RegisterResources(SessionManager container)
        {
            for (int i = 0; i < RegisteredResources.Length; i++)
            {
                container.Register<ILogger>(
                    RegisteredResources[i].Type!,
                    RegisteredResources[i].Name!,
                    isSingleton: false);
            }
        }

    }
}
