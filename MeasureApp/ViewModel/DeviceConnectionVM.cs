using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.ViewModel
{
    public partial class DeviceConnectionVM : BaseVM
    {
        [ObservableProperty]
        private bool isSelectedDeviceConnected = false;

    }
}
