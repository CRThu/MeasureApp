using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class GpibDeviceName : INotifyPropertyChanged
    {
        private string _DeviceName = string.Empty;
        public string DeviceName
        {
            get => DeviceName;
            set
            {
                if (_DeviceName != value)
                {
                    _DeviceName = value;
                    OnPropertyChanged("DeviceName");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DeviceName"));
        }

        public GpibDeviceName(string deviceName)
        {
            DeviceName = deviceName;
        }
    }
}
