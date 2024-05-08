using CarrotProtocolLib.Device;
using IOStreamDemo.Streams;
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
                new DeviceInfo("GPIB","GPIB://701","NI VISA GPIB 701 FOR TEST"),
                new DeviceInfo("GPIB","GPIB://702","NI VISA GPIB 702 FOR TEST"),
            ];
        }
    }
}
