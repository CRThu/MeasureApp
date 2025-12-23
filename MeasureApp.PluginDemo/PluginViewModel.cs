using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.PluginDemo
{
    public partial class PluginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title = "插件Demo";

        [ObservableProperty]
        private string hello = "Helloworld";
    }
}
