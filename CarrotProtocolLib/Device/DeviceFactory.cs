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
        public static IDevice GetDevice(string deviceName)
        {
            return deviceName switch
            {
                "GeneralBufferedDevice" => new GeneralBufferedDevice(),
                _ => throw new NotImplementedException()
            };
        }

        public static IDriver GetDriver(string driverName)
        {
            return driverName switch
            {
                "SerialPortDriver" => new SerialPortDriver(),
                "FtdiD2xxDriver" => new FtdiD2xxDriver(),
                _ => throw new NotImplementedException()
            };
        }

        public static ILogger GetLogger(string loggerName)
        {
            return loggerName switch
            {
                "ProtocolLogger" => new ProtocolLogger(),
                _ => throw new NotImplementedException()
            };
        }

        public static DeviceDataReceiveService GetReceiveService(string receiveServiceName)
        {
            return receiveServiceName switch
            {
                "DeviceDataReceiveService" => new DeviceDataReceiveService(),
                _ => throw new NotImplementedException()
            };
        }

        public static ProtocolDecodeBaseService GetDecodeService(string decodeServiceName)
        {
            return decodeServiceName switch
            {
                "CarrotDataProtocolDecodeService" => new CarrotDataProtocolDecodeService(),
                "RawAsciiProtocolDecodeService" => new RawAsciiProtocolDecodeService(),
                _ => throw new NotImplementedException()
            };
        }

        public static IDevice Create(
            string deviceName, string driverName, string loggerName,
            string receiveServiceName, string decodeServiceName)
        {
            // device initial
            var device = GetDevice(deviceName);
            var driver = GetDriver(driverName);
            var logger = GetLogger(loggerName);
            var receiveService = GetReceiveService(receiveServiceName);
            var decodeService = GetDecodeService(decodeServiceName);

            device.Driver = driver;
            device.Logger = logger;
            device.DataReceiveService = receiveService;
            device.ProtocolDecodeService = decodeService;

            // Driver initial
            // driverName.ErrorReceived

            // Logger initial
            // reserved

            // DataReceiveService initial
            receiveService.Device = device;

            // ProtocolDecodeService initial
            decodeService.Device = device;
            decodeService.Logger = logger;
            // decodeService.ProtocolDecodeError


            return device;
        }
    }
}
