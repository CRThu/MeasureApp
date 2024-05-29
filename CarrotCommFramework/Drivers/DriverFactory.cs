using CarrotCommFramework.Loggers;
using CarrotCommFramework.Protocols;
using CarrotCommFramework.Sessions;
using CarrotCommFramework.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CarrotCommFramework.Drivers
{
    public class DriverFactory
    {

        private static readonly DriverFactory current = new();
        public static DriverFactory Current => current;

        public Dictionary<string, IDriver> Drivers { get; private set; } = new();

        public DriverFactory()
        {
            Create("COM", "COM");
            Create("GPIB", "GPIB");
        }

        public IDriver Create(string serviceKey, string instanceKey)
        {
            serviceKey = serviceKey.ToUpper();
            if (serviceKey == "COM")
                return new SerialDriver(instanceKey);
            else if (serviceKey == "GPIB")
                return new GpibDriver(instanceKey);
            else
                throw new NotImplementedException($"No Driver {serviceKey}.");
        }

        public IDriver Get(string serviceKey, string instanceKey)
        {
            if (Drivers.TryGetValue(instanceKey, out var instance))
                return instance;
            else
            {
                var service = Create(serviceKey!, instanceKey);
                Drivers.Add(instanceKey, service);
                return service;
            }
        }

        /// <summary>
        /// 获取设备名称
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public DeviceInfo[] FindDevices(string filter = null)
        {
            List<DeviceInfo> devicesFound = [];
            var drivers = Drivers;
            foreach (var driver in drivers)
            {
                var devs = driver.Value.FindDevices();
                devicesFound.AddRange(devs);
            }
            return devicesFound.ToArray();
        }
    }
}
