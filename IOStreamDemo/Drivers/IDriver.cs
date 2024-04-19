using CarrotProtocolLib.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Drivers
{
    public interface IDriver
    {
        public DeviceInfo[] FindDevices();

    }
}
