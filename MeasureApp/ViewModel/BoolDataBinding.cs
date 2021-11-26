using MeasureApp.Model;
using MeasureApp.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp
{
    public class BoolDataBinding : NotificationObjectBase
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
