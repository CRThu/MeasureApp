using MeasureApp.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeasureApp.View.Pages;

namespace MeasureApp.ViewModel
{
    public class MainWindowPages : NotificationObjectBase
    {
        private DevicesConnectionPage devicesConnectionPage;
        public DevicesConnectionPage DevicesConnectionPage
        {
            get => devicesConnectionPage;
            set
            {
                devicesConnectionPage = value;
                RaisePropertyChanged(() => DevicesConnectionPage);
            }
        }

        private Multimeter3458AControlPage multimeter3458AControlPage;
        public Multimeter3458AControlPage Multimeter3458AControlPage
        {
            get => multimeter3458AControlPage;
            set
            {
                multimeter3458AControlPage = value;
                RaisePropertyChanged(() => Multimeter3458AControlPage);
            }
        }



        public MainWindowPages(object dataContext)
        {
            DevicesConnectionPage = new DevicesConnectionPage(dataContext);
            Multimeter3458AControlPage = new Multimeter3458AControlPage(dataContext);
        }
    }
}
