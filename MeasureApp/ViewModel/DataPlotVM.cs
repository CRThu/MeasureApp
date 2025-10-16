using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DryIoc;
using MeasureApp.Messages;
using MeasureApp.Model.Common;
using MeasureApp.Services;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.ViewModel
{
    public partial class DataPlotVM : BaseVM
    {
        private readonly AppContextManager _context;
        public AppContextManager Context => _context;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Plot))]
        private string selectedDataXKey;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Plot))]
        private string selectedDataYKey;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Plot))]
        private bool isCustomDataXUsed = false;

        [ObservableProperty]
        private bool isAutoUpdateEnabled = true;

        [ObservableProperty]
        private Plot plot;

        public DataPlotVM(AppContextManager context)
        {
            _context = context;
        }

        [RelayCommand]
        public void ManualRefresh()
        {
            try
            {
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }
    }
}
