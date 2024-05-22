using IOStreamDemo.Drivers;
using IOStreamDemo.Sessions;
using IOStreamDemo.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Protocols
{
    public class ProtocolResourceInfo(string name, Type type)
    {
        public string? Name { get; set; } = name;
        public Type? Type { get; set; } = type;
    }

    public class ProtocolManager
    {
        public static ProtocolResourceInfo[] RegisteredResources =
        [
            new ProtocolResourceInfo("RAPV1", typeof(RawAsciiProtocol)),
            new ProtocolResourceInfo("CDPV1", typeof(CarrotDataProtocol)),
        ];

        public static void RegisterResources(SessionManager container)
        {
            for (int i = 0; i < RegisteredResources.Length; i++)
            {
                container.Register<IProtocol>(
                    RegisteredResources[i].Type!,
                    RegisteredResources[i].Name!,
                    isSingleton: false);
            }
        }

    }
}
