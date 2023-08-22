using CarrotProtocolLib.Driver;
using CarrotProtocolLib.Logger;
using CarrotProtocolLib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Device
{
    /// <summary>
    /// 工厂类
    /// </summary>
    public static class DeviceFactory
    {
        public static IDevice GetDevice(string deviceName, string driverName, string decodeProtocol,ILogger logger)
        {
            return deviceName switch
            {
                "GeneralBufferedDevice" => new GeneralBufferedDevice(driverName, decodeProtocol, logger),
                _ => throw new NotImplementedException()
            };
        }
    }
}
