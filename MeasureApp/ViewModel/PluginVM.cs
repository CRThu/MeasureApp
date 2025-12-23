using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DryIoc;
using MeasureApp.Messages;
using MeasureApp.Plugins.Interfaces;
using MeasureApp.Services;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.ViewModel
{
    public partial class PluginVM : BaseVM
    {
        private readonly PluginService _pluginService;

        [ObservableProperty]
        public IMeasureAppPlugin selectedPlugin;

        public PluginVM()
        {
            Title = "Plugin";
            ContentId = "PluginVM";

            _pluginService = App.Locator.PluginSrv;
        }

        [RelayCommand]
        private void Open()
        {
            try
            {
                _pluginService.LoadPlugins(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins"));

                if (SelectedPlugin == null)
                {
                    SelectedPlugin = _pluginService.Plugins.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
