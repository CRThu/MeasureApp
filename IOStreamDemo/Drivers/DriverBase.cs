using CarrotCommFramework.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Drivers
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
