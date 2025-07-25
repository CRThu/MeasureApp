﻿using MeasureApp.Model.Common;
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
        public CommandLogService CommandLogger { get; }
        public DataLogService DataLogger { get; }

        public AppContextManager(DeviceManager devices, ConfigManager configs, AppLogService appLogger, CommandLogService commandLogger, DataLogService dataLogger)
        {
            Devices = devices;
            Configs = configs;
            AppLogger = appLogger;
            CommandLogger = commandLogger;
            DataLogger = dataLogger;
        }

        public void Dispose()
        {
            Devices?.Dispose();
        }
    }
}
