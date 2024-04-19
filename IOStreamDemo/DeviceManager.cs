using CarrotProtocolLib.Device;
using IOStreamDemo.Drivers;
using IOStreamDemo.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo
{
    public class DeviceManager
    {
        public static (string name, IDriver type)[] RegisteredResources =
        [
            //("TCP", typeof(TcpStream) ),
            ("SERIAL", new SerialDriver() ),
            ("GPIB", new GpibDriver() ),
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
                var devs = RegisteredResources[i].type.FindDevices();
                devices.AddRange(devs);
            }
            return devices.ToArray();
        }

        public static void CreateSession(string address, string logger, string service)
        {
            if (address.StartsWith("com://"))
            {
            }
            else
                return;
        }
    }
}
