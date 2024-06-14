using CarrotCommFramework.Streams;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Drivers
{
    public class SerialDriver : DriverBase
    {
        public SerialDriver()
        {
            Name = "COM";
        }


        public override DeviceInfo[] FindDevices()
        {
            return SerialPort.GetPortNames().Select(d => new DeviceInfo("COM", d, "串口设备")).ToArray();
            //return
            //[
            //    new DeviceInfo("COM","200","SERIALPORT COM200 FOR TEST"),
            //    new DeviceInfo("COM","201","SERIALPORT COM201 FOR TEST")
            //];
        }
    }
}
