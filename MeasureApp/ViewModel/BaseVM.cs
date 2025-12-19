using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.ViewModel
{
    public partial class BaseVM : ObservableObject
    {
        [ObservableProperty]
        private bool isVisible = true;

        [ObservableProperty]
        private string contentId;

        [ObservableProperty]
        private bool isClosed;

        [ObservableProperty]
        private bool canClose;

        [ObservableProperty]
        private string title;


        public BaseVM()
        {
            CanClose = true;
            IsClosed = false;
        }

        [RelayCommand]
        public void Close()
        {
            IsClosed = true;
        }
    }
}
