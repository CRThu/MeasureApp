using DryIoc;
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
            container.RegisterMany<DeviceStatusVM>(Reuse.Singleton);
            container.RegisterMany<DeviceConnectionVM>(Reuse.Singleton);
            container.RegisterMany<DeviceDebugVM>(Reuse.Singleton);
            container.RegisterMany<DeviceLogVM>(Reuse.Singleton);
            container.RegisterMany<DataMonitorVM>(Reuse.Singleton);
            container.RegisterMany<DataPlotVM>(Reuse.Singleton);
            container.RegisterMany<ScriptRunnerVM>(Reuse.Singleton);
            container.RegisterMany<RegisterEditorVM>(Reuse.Singleton);
            container.RegisterMany<AboutAppVM>(Reuse.Singleton);
            container.RegisterMany<AppLogVM>(Reuse.Singleton);
            container.RegisterMany<DataProcVM>(Reuse.Singleton);
            container.RegisterMany<UIDebugVM>(Reuse.Singleton);

            container.Register<AppContextManager>(Reuse.Singleton);
            container.Register<DeviceManager>(Reuse.Singleton);
            container.Register<ConfigManager>(Reuse.Singleton);
            container.Register<AppLogService>(Reuse.Singleton);
            container.Register<CommandLogService>(Reuse.Singleton);
            container.Register<DataLogService>(Reuse.Singleton);
            container.Register<RegisterLogService>(Reuse.Singleton);
        }

        public AppContextManager Context => container.Resolve<AppContextManager>();


        public MainWindowVM MainWindow => container.Resolve<MainWindowVM>();
        public DeviceStatusVM DeviceStatus => container.Resolve<DeviceStatusVM>();
        public DeviceConnectionVM DeviceConnection => container.Resolve<DeviceConnectionVM>();
        public DeviceDebugVM DeviceDebug => container.Resolve<DeviceDebugVM>();
        public DeviceLogVM DeviceLog => container.Resolve<DeviceLogVM>();
        public DataMonitorVM DataMonitor => container.Resolve<DataMonitorVM>();
        public DataPlotVM DataPlot => container.Resolve<DataPlotVM>();
        public ScriptRunnerVM ScriptRunner => container.Resolve<ScriptRunnerVM>();
        public RegisterEditorVM RegisterEditor => container.Resolve<RegisterEditorVM>();
        public AboutAppVM AboutApp => container.Resolve<AboutAppVM>();
        public AppLogVM AppLog => container.Resolve<AppLogVM>();

        public DataProcVM DataProc => container.Resolve<DataProcVM>();
        public UIDebugVM UIDebug => container.Resolve<UIDebugVM>();

    }
}
