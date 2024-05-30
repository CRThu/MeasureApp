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
        public string Name { get; set; }
        public DeviceInfo[] FindDevices();
    }

    public abstract class DriverBase : IDriver
    {
        public string Name { get; set; }
        public abstract DeviceInfo[] FindDevices();
    }
}
