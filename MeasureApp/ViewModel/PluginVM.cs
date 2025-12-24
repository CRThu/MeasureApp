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
        private readonly PluginService _pluginSvc;
        public PluginService PluginSvc => _pluginSvc;


        [ObservableProperty]
        public IMeasureAppPlugin selectedPlugin;

        public PluginVM(PluginService pluginSvc)
        {
            Title = "Plugin";
            ContentId = "PluginVM";

            _pluginSvc = pluginSvc;
        }

        [RelayCommand]
        private void Open()
        {
            try
            {
                PluginSvc.LoadPlugins(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins"));

                if (SelectedPlugin == null)
                {
                    SelectedPlugin = PluginSvc.Plugins.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
