using CarrotProtocolLib.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Drivers
{
    public class GpibDriver : IDriver
    {
        public GpibDriver()
        {
        }

        public DeviceInfo[] FindDevices()
        {
            return
            [
                new DeviceInfo("GPIB","GPIB722","ni visa gpib 722 test")
            ];
        }
    }
}
