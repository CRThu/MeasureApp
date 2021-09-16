using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class StringDataBinding : INotifyPropertyChanged
    {
        private string _stringData;
        public event PropertyChangedEventHandler PropertyChanged;
        public string StringData
        {
            get => _stringData;
            set
            {
                //if (value != _stringData)
                //{
                _stringData = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("StringData"));
                }
                //}
            }
        }
    }
}
