using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Services.Script
{
    public partial class ObservableKeyValuePair : ObservableObject
    {
        [ObservableProperty]
        private string key;

        [ObservableProperty]
        private string value;
    }
}
