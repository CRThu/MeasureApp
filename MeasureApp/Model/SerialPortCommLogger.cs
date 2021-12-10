using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model
{
    public class SerialPortCommLogger : NotificationObjectBase
    {
        // TODO 封装LOG
        private object logCollectionLocker = new();
        private ObservableRangeCollection<SerialPortCommLog> logCollection = new();
        public ObservableRangeCollection<SerialPortCommLog> LogCollection
        {
            get => logCollection;
            set
            {
                logCollection = value;
                RaisePropertyChanged(() => LogCollection);
            }
        }


    }
}
