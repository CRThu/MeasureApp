using MeasureApp.Services.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.PluginDemo
{
    public class PluginViewEntry : IPlugin
    {
        public string Title => "插件Demo";
        public object View => new PluginView();
    }
}
