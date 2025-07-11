using MeasureApp.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Services
{
    public class AppContextManager
    {
        public DeviceManager Devices { get; }
        public ConfigManager Configs { get; }
        public AppLogService Logger { get; }

        public AppContextManager(DeviceManager devices, ConfigManager configs, AppLogService logger)
        {
            Devices = devices;
            Configs = configs;
            Logger = logger;
        }
    }
}
