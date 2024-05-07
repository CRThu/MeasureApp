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
        public static DeviceInfo[] FindDevices(string filter)
        {
            List<DeviceInfo> devices = new();
            for (int i = 0; i < RegisteredResources.Length; i++)
            {
                //var devs = RegisteredResources[i].type.FindDevices();
                //devices.AddRange(devs);
            }
            return devices.ToArray();
        }

        public static void CreateSession(SessionContainer container, string address, string logger, string service)
        {

            // ADDRESS
            // COM://7@9600
            // TCP://127.0.0.1:8888
            // GPIB://22

            // LOGGER
            // CONSOLE://1

            // SERVICE
            // CDPV1

            string[] addrInfo = address.ToUpper().Split("://", 2);
            string loggerKey = logger.ToUpper();

            if (addrInfo.Length != 2)
                throw new NotImplementedException();

            var streamType = RegisteredResources.Where(res => res.Name == addrInfo[0]).FirstOrDefault()!.StreamType;

            var s = container.GetOrCreate<IDriverCommStream>(streamType!, address);
            var l = container.GetOrCreate<Loggers.ILogger>(typeof(ConsoleLogger), loggerKey);

            s.Address = address;
            s.LoggerKey = loggerKey;
        }
    }
}
