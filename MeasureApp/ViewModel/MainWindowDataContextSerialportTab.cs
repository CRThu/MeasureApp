using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // DataGrid数据绑定
        public dynamic SerialPortStorageDataGridBinding
        {
            get => DataStorageInstance.DataStorageDictionary[KeySerialPortString];
            set
            {
                DataStorageInstance.DataStorageDictionary[KeySerialPortString] = value;
                RaisePropertyChanged(() => SerialPortStorageDataGridBinding);
            }
        }
    }
}
