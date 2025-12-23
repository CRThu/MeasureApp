using MeasureApp.Plugins.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.PluginDemo
{
    public class PluginDemo : IMeasureAppPlugin
    {
        public string Name { get; set; }

        public FrameworkElement View { get; set; }

        public PluginDemo(string name, FrameworkElement view)
        {
            Name = name;
            View = view;
        }
    }

    public class PluginViewEntry : IMeasureAppPluginFactory
    {
        public IMeasureAppPlugin CreatePluginView(IMeasureAppService service)
        {
            var vm = new PluginViewModel(service);
            var v = new PluginView();
            v.DataContext = vm;
            return new PluginDemo("插件Demo", v);
        }
    }
}
