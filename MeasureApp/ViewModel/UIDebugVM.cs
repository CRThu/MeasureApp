using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DryIoc;
using MeasureApp.Messages;
using MeasureApp.Services;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.ViewModel
{
    public partial class UIDebugVM : BaseVM
    {
        [ObservableProperty]
        private IToolViewModel selectedTool;

        public UIDebugVM()
        {
            Title = "UI调试";
            ContentId = "UIDebug";
        }

        [RelayCommand]
        private void UpdateDocument()
        {
        }

        [RelayCommand]
        private void UpdateAnchorable()
        {
        }
    }
}
