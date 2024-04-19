using CarrotProtocolLib.Device;
using System;
using System.Collections.Generic;
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
            return
            [
                new DeviceInfo("SERIAL","COM999","serial port com test")
            ];
        }
    }
}
