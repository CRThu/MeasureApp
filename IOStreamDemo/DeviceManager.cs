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
            ("COM", new SerialDriver() ),
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

        public static void CreateSession(SessionContainer container, string address, string logger, string service)
        {
            // COM://7
            // TCP://127.0.0.1:8888
            // GPIB://22

            string[] addrInfo = address.ToUpper().Split("://", 2);
            
            if (addrInfo.Length != 2)
                throw new NotImplementedException();

            //container.
        }
    }
}
