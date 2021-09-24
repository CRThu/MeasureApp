using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class BoolDataBinding : ViewModelBase
    {
        private bool _boolData;
        public bool BoolData
        {
            get => _boolData;
            set
            {
                _boolData = value;
                RaisePropertyChanged(() => BoolData);
            }
        }

        public BoolDataBinding Clone()
        {
            return new BoolDataBinding
            {
                BoolData = BoolData
            };
        }
    }
}
