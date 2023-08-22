using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolCommDemo.Logger
{
    public partial class DataStorageValue<T> : ObservableObject
    {
        [ObservableProperty]
        private T? value = default;
    }
}
