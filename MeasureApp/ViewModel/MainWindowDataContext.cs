using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.ViewModel
{
    public class MainWindowDataContext : NotificationObjectBase
    {
        private string _autoTextBox;
        public string AutoTextBox
        {
            get => _autoTextBox;
            set
            {
                _autoTextBox = value;
                RaisePropertyChanged(() => AutoTextBox);
            }
        }
    }
}
