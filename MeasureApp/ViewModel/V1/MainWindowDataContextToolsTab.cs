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
using MeasureApp.Model.Common;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 设置
        private BitsOperationModel bitsModel = new();
        public BitsOperationModel BitsModel
        {
            get => bitsModel;
            set
            {
                bitsModel = value;
                RaisePropertyChanged(() => BitsModel);
            }
        }

    }
}
