﻿using DryIoc;
using MeasureApp.Services;
using MeasureApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.ViewModel
{
    public class ViewModelLocator
    {
        private readonly Container container;

        public ViewModelLocator()
        {
            container = new Container();

            // Register
            container.Register<MainWindowVM>(Reuse.Singleton);
            container.Register<DeviceConnectionVM>(Reuse.Singleton);
            container.Register<DeviceDebugVM>(Reuse.Singleton);
            container.Register<DataMonitorVM>(Reuse.Singleton);
            container.Register<DataProcVM>(Reuse.Singleton);
            container.Register<ScriptRunnerVM>(Reuse.Singleton);
            // todo
            container.Register<AppLogVM>(Reuse.Singleton);
            container.Register<DataPlotVM>(Reuse.Singleton);
            container.Register<AboutAppVM>(Reuse.Singleton);
            container.Register<RegisterEditorVM>(Reuse.Singleton);

            container.Register<AppContextManager>(Reuse.Singleton);
            container.Register<DeviceManager>(Reuse.Singleton);
            container.Register<ConfigManager>(Reuse.Singleton);
            container.Register<AppLogService>(Reuse.Singleton);
            container.Register<CommandLogService>(Reuse.Singleton);
            container.Register<DataLogService>(Reuse.Singleton);
        }

        public AppContextManager Context => container.Resolve<AppContextManager>();


        public MainWindowVM MainWindow => container.Resolve<MainWindowVM>();
        public DeviceConnectionVM DeviceConnection => container.Resolve<DeviceConnectionVM>();
        public DeviceDebugVM DeviceDebug => container.Resolve<DeviceDebugVM>();
        public DataMonitorVM DataMonitor => container.Resolve<DataMonitorVM>();
        public DataProcVM DataProc => container.Resolve<DataProcVM>();
        public ScriptRunnerVM ScriptRunner => container.Resolve<ScriptRunnerVM>();

        public AppLogVM AppLog => container.Resolve<AppLogVM>();
        public DataPlotVM DataPlot => container.Resolve<DataPlotVM>();

        public AboutAppVM AboutApp => container.Resolve<AboutAppVM>();
        public RegisterEditorVM RegisterEditor => container.Resolve<RegisterEditorVM>();
    }
}
