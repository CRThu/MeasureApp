using NationalInstruments.VisaNS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            try
            {
                string expression = "?*";
                //string expression = "GPIB?*INSTR";
                string[] res = ResourceManager.GetLocalManager().FindResources(expression);
                return res.Select(d => new DeviceInfo("VISA", d, "NI-VISA DEVICE")).ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return Array.Empty<DeviceInfo>();
            }
        }
    }
}
