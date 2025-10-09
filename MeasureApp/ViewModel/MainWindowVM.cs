using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DryIoc;
using MeasureApp.Model.Common;
using MeasureApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowVM : BaseVM
    {
        private readonly AppContextManager _context;

        [ObservableProperty]
        private string statusBarText = "Text from MainWindowVM";

        public MainWindowVM(AppContextManager context)
        {
            _context = context;
        }

        [RelayCommand]
        public void MainWindowLoaded()
        {
            try
            {
                //MessageBox.Show("MainWindowLoaded");
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void MainWindowClosing()
        {
            try
            {
                _context.Configs.AppConfig.PresetCommands = App.Locator.DeviceDebug.Presets;
                _context.Configs.Update();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void MainWindowClosed()
        {
            try
            {
                _context?.Dispose();
                //MessageBox.Show("MainWindowClosed");
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

    }
}
