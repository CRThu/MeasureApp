using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class StringDataBinding : ViewModelBase
    {
        private string _stringData;
        public string StringData
        {
            get => _stringData;
            set
            {
                _stringData = value;
                RaisePropertyChanged(() => StringData);
            }
        }

        public StringDataBinding Clone()
        {
            return new StringDataBinding
            {
                StringData = StringData
            };
        }
    }
}
