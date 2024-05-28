using CarrotProtocolLib.Device;
using IOStreamDemo.Streams;
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

    public abstract class DriverBase : IDriver
    {
        public abstract DeviceInfo[] FindDevices();
    }
}
