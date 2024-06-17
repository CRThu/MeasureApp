using CarrotCommFramework.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Drivers
{
    public class NiVisaDriver : DriverBase
    {
        public NiVisaDriver()
        {
            Name = "VISA";
        }

        public override DeviceInfo[] FindDevices()
        {
            return
            [
                new DeviceInfo("VISA","701","NI VISA GPIB 701 FOR TEST"),
                new DeviceInfo("VISA","702","NI VISA GPIB 702 FOR TEST"),
            ];
        }
    }
}
