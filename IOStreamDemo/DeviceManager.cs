using CarrotProtocolLib.Device;
using CarrotProtocolLib.Driver;
using CarrotProtocolLib.Logger;
using DryIoc;
using IOStreamDemo.Drivers;
using IOStreamDemo.Loggers;
using IOStreamDemo.Services;
using IOStreamDemo.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IOStreamDemo
{
    public class DeviceResourcesInfo(string name, Type driverType, Type streamType)
    {
        public string? Name { get; set; } = name;
        public Type? DriverType { get; set; } = driverType;
        public Type? StreamType { get; set; } = streamType;
    }

    public class DeviceManager
    {
        public static DeviceResourcesInfo[] RegisteredResources =
        [
            new DeviceResourcesInfo("COM", typeof(SerialDriver), typeof(SerialStream)),
            new DeviceResourcesInfo("GPIB", typeof(GpibDriver), typeof(VisaGpibStream)),
            //("TCP", typeof(TcpStream) ),
            //("FTDI", typeof(FtdiSyncFifoStream) ),
        ];

        /// <summary>
        /// 获取设备名称
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static DeviceInfo[] FindDevices(SessionManager container, string filter = null)
        {
            List<DeviceInfo> devicesFound = [];
            var drivers = container.Container.Resolve<Drivers.IDriver[]>();
            for (int i = 0; i < drivers.Length; i++)
            {
                var devs = drivers[i].FindDevices();
                devicesFound.AddRange(devs);
            }
            return devicesFound.ToArray();
        }

        public static void RegisterResources(SessionManager container)
        {
            for (int i = 0; i < RegisteredResources.Length; i++)
            {
                container.Register(typeof(Drivers.IDriver),
                    RegisteredResources[i].DriverType!,
                    RegisteredResources[i].Name!, isSingleton: true);
                container.Register(typeof(IDriverCommStream),
                    RegisteredResources[i].StreamType!,
                    RegisteredResources[i].Name!, isSingleton: false);
            }
        }

        public static Session CreateSession(SessionManager container, string address, string logger, string protocol)
        {

            // ADDRESS
            // COM://7@9600
            // TCP://127.0.0.1:8888
            // GPIB://22

            // LOGGER
            // CONSOLE://1

            // SERVICE
            // CDPV1

            string[] devInfo = address.ToUpper().Split("://", 2);
            string[] loggerInfo = logger.ToUpper().Split("://", 2);
            string protocolInfo = protocol.ToUpper();

            if (devInfo.Length != 2 || loggerInfo.Length != 2)
                throw new NotImplementedException();

            var resName = devInfo[0];
            var loggerName = loggerInfo[0];

            var s = container.Add(address, resName, loggerName, protocolInfo);

            return s;
        }
    }
}
