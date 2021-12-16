using MeasureApp.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 设置
        private int bits = 0;
        public int Bits
        {
            get => bits;
            set
            {
                bits = value;
                RaisePropertyChanged(() => Bits);
            }
        }

    }
}
