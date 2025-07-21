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
            container.Register<MainWindowVM>();
            container.Register<DeviceConnectionVM>();
            container.Register<DeviceDebugVM>();
            container.Register<DataMonitorVM>();

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
    }
}
