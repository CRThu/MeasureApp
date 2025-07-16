using MeasureApp.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Services
{
    public class AppContextManager : IDisposable
    {
        public DeviceManager Devices { get; }
        public ConfigManager Configs { get; }
        public AppLogService AppLogger { get; }
        public SessionLogService SessionLogger { get; }

        public AppContextManager(DeviceManager devices, ConfigManager configs, AppLogService appLogger, SessionLogService sessionLogger)
        {
            Devices = devices;
            Configs = configs;
            AppLogger = appLogger;
            SessionLogger = sessionLogger;
        }

        public void Dispose()
        {
            Devices?.Dispose();
        }
    }
}
