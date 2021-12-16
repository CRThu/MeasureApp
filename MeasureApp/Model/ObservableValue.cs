using MeasureApp.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model
{
    public class ObservableValue : NotificationObjectBase
    {
        private dynamic value;
        public dynamic Value
        {
            get => value;
            set
            {
                this.value = value;
                RaisePropertyChanged(() => Value);
            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
