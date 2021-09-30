using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp
{
    public class StringDataClass : NotificationObjectBase
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

        public StringDataClass Clone()
        {
            return new StringDataClass
            {
                StringData = StringData
            };
        }
    }
}
