using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.ViewModel
{
    public partial class BaseVM : ObservableObject, IToolViewModel
    {
        [ObservableProperty]
        private bool isVisible = true;

        [ObservableProperty]
        private string contentId;

        [ObservableProperty]
        private bool canClose;

        [ObservableProperty]
        private string title;


        public BaseVM()
        {
            CanClose = true;
            IsVisible = true;
        }

        [RelayCommand]
        public void Close()
        {
            IsVisible = false;
        }
    }
}
