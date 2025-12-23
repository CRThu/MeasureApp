using CommunityToolkit.Mvvm.ComponentModel;
using DryIoc;
using MeasureApp.Services.Plugin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Services
{
    public partial class PluginService : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<IPlugin> plugins = new();

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

                var pluginTypes = assembly.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var type in pluginTypes)
                {
                    App.Locator.Container.Register(typeof(IPlugin), type, serviceKey: type.FullName);

                    var plugin = (IPlugin)App.Locator.Container.Resolve(typeof(IPlugin), serviceKey: type.FullName);
                    Plugins.Add(plugin);
                }
            }
        }
    }
}
