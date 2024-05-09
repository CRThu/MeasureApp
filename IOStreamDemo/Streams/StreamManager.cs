using IOStreamDemo.Drivers;
using IOStreamDemo.Loggers;
using IOStreamDemo.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Streams
{
    public class StreamResourceInfo(string name, Type type)
    {
        public string? Name { get; set; } = name;
        public Type? Type { get; set; } = type;
    }

    public class StreamManager
    {
        public static StreamResourceInfo[] RegisteredResources =
        [
            new StreamResourceInfo("COM", typeof(SerialStream)),
            new StreamResourceInfo("GPIB", typeof(VisaGpibStream)),
        ];

        public static void RegisterResources(SessionManager container)
        {
            for (int i = 0; i < RegisteredResources.Length; i++)
            {
                container.Register<IDriverCommStream>(
                    RegisteredResources[i].Type!,
                    RegisteredResources[i].Name!,
                    isSingleton: false);
            }
        }

    }
}
