using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model
{
    public class ObservableTValue<T> : NotificationObjectBase
    {
        private T value;
        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                RaisePropertyChanged(() => Value);
                if (OnValueChanged is not null)
                    OnValueChanged(this, new EventArgs());
            }
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public delegate void ValueChanged(object sender, EventArgs e);
        public event ValueChanged OnValueChanged;
    }
}
