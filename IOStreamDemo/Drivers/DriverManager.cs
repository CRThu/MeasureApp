using CarrotProtocolLib.Device;
using CarrotProtocolLib.Driver;
using CarrotProtocolLib.Logger;
using DryIoc;
using IOStreamDemo.Loggers;
using IOStreamDemo.Services;
using IOStreamDemo.Sessions;
using IOStreamDemo.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IOStreamDemo.Drivers
{
    public class DriverResourcesInfo(string name, Type type)
    {
        public string? Name { get; set; } = name;
        public Type? Type { get; set; } = type;
    }

    public class DriverManager
    {
        public static DriverResourcesInfo[] RegisteredResources =
        [
            new DriverResourcesInfo("COM", typeof(SerialDriver)),
            new DriverResourcesInfo("GPIB", typeof(GpibDriver)),
        ];

        /// <summary>
        /// 获取设备名称
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static DeviceInfo[] FindDevices(SessionManager container, string filter = null)
        {
            List<DeviceInfo> devicesFound = [];
            var drivers = container.Drivers;
            foreach (var driver in drivers)
            {
                var devs = driver.Value.FindDevices();
                devicesFound.AddRange(devs);
            }
            return devicesFound.ToArray();
        }

        public static void RegisterResources(SessionManager container)
        {
            for (int i = 0; i < RegisteredResources.Length; i++)
            {
                container.Register<IDriver>(
                    RegisteredResources[i].Type!,
                    RegisteredResources[i].Name!,
                    isSingleton: true);
            }
        }

    }
}
