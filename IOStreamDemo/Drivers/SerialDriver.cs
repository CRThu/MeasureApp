using CarrotProtocolLib.Device;
using IOStreamDemo.Streams;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Drivers
{
    public class SerialDriver : IDriver
    {
        public SerialDriver()
        {
        }

        public DeviceInfo[] FindDevices()
        {
            return SerialPort.GetPortNames().Select(d => new DeviceInfo("SERIAL", $"COM://{d[3..]}", "串口设备")).ToArray();
            //return
            //[
            //    new DeviceInfo("SERIAL","COM://200","SERIALPORT COM200 FOR TEST"),
            //    new DeviceInfo("SERIAL","COM://201","SERIALPORT COM201 FOR TEST")
            //];
        }
    }
}
