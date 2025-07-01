using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DryIoc;
using MeasureApp.Model.Common;
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
        [ObservableProperty]
        private string statusBarText= "Text from MainWindowVM";

        [RelayCommand]
        public void MainWindowLoaded()
        {
            try
            {
                MessageBox.Show("MainWindowLoaded");
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
                MessageBox.Show("MainWindowClosed");
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

    }
}
