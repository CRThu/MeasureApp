using DryIoc;
using MeasureApp.Services;
using MeasureApp.View;
using MeasureApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp
{
    public class Locator
    {
        private readonly Container container;
        public Container Container => container;

        public Locator()
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
            container.RegisterMany<PluginVM>(Reuse.Singleton);

            container.Register<AppContextManager>(Reuse.Singleton);
            container.Register<DeviceManager>(Reuse.Singleton);
            container.Register<ConfigManager>(Reuse.Singleton);
            container.Register<PluginService>(Reuse.Singleton);
            container.RegisterMany<MeasureAppService>(Reuse.Singleton);
            container.RegisterMany<AppLogService>(Reuse.Singleton);
            container.RegisterMany<CommandLogService>(Reuse.Singleton);
            container.RegisterMany<DataLogService>(Reuse.Singleton);
            container.RegisterMany<RegisterLogService>(Reuse.Singleton);

        }

        public DeviceManager DeviceManager => container.Resolve<DeviceManager>();
        public ConfigManager ConfigManager => container.Resolve<ConfigManager>();
        public PluginService PluginSvc => container.Resolve<PluginService>();
        public MeasureAppService AppSvc => container.Resolve<MeasureAppService>();
        public AppLogService AppLogger => container.Resolve<AppLogService>();
        public CommandLogService CommandLogger => container.Resolve<CommandLogService>();
        public DataLogService DataLogger => container.Resolve<DataLogService>();
        public RegisterLogService RegisterLogger => container.Resolve<RegisterLogService>();



        public MainWindowVM MainWindowVM => container.Resolve<MainWindowVM>();
        public DeviceStatusVM DeviceStatusVM => container.Resolve<DeviceStatusVM>();
        public DeviceConnectionVM DeviceConnectionVM => container.Resolve<DeviceConnectionVM>();
        public DeviceDebugVM DeviceDebugVM => container.Resolve<DeviceDebugVM>();
        public DeviceLogVM DeviceLogVM => container.Resolve<DeviceLogVM>();
        public DataMonitorVM DataMonitorVM => container.Resolve<DataMonitorVM>();
        public DataPlotVM DataPlotVM => container.Resolve<DataPlotVM>();
        public ScriptRunnerVM ScriptRunnerVM => container.Resolve<ScriptRunnerVM>();
        public RegisterEditorVM RegisterEditorVM => container.Resolve<RegisterEditorVM>();
        public AboutAppVM AboutAppVM => container.Resolve<AboutAppVM>();
        public AppLogVM AppLogVM => container.Resolve<AppLogVM>();

        public DataProcVM DataProcVM => container.Resolve<DataProcVM>();
        public UIDebugVM UIDebugVM => container.Resolve<UIDebugVM>();

        public PluginVM PluginVM => container.Resolve<PluginVM>();
    }
}
