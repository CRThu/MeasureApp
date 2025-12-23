using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MeasureApp.Plugins.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.PluginDemo
{
    public partial class PluginViewModel : ObservableObject
    {
        private readonly IMeasureAppService _service;

        [ObservableProperty]
        private string title = "插件Demo";

        [ObservableProperty]
        private string hello = "Helloworld";

        public PluginViewModel(IMeasureAppService service)
        {
            _service = service;
        }

        [RelayCommand]
        private void Run()
        {
            _service.Send($"Msg from {nameof(PluginViewModel)}");
        }
    }
}
