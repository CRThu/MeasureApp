using CommunityToolkit.Mvvm.ComponentModel;
using DryIoc;
using MeasureApp.Plugins.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.Services
{
    public partial class PluginService : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<IMeasureAppPlugin> plugins = new();

        public PluginService()
        {

        }

        public void LoadPlugins(string dir)
        {
            if (!Directory.Exists(dir))
                return;

            var dlls = Directory.GetFiles(dir, "*.dll");

            foreach (var dll in dlls)
            {
                var assembly = Assembly.LoadFrom(dll);

                var factoryTypes = assembly.GetTypes().Where(t => typeof(IMeasureAppPluginFactory).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var factoryType in factoryTypes)
                {
                    App.Locator.Container.Register(typeof(IMeasureAppPluginFactory), factoryType, serviceKey: factoryType.FullName);

                    var factory = (IMeasureAppPluginFactory)App.Locator.Container.Resolve(typeof(IMeasureAppPluginFactory), serviceKey: factoryType.FullName);

                    var plugin = factory.CreatePluginView(App.Locator.AppSvc);
                    Plugins.Add(plugin);
                }
            }
        }
    }
}
