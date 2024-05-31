using CommunityToolkit.Mvvm.ComponentModel;
using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolCommDemo.ViewModel
{
    public class ViewModelLocator
    {
        private Container container;

        public ViewModelLocator()
        {
            container = new Container();
            container.Register<MainViewModel>(Reuse.Singleton);
        }

        public ViewModelBase? Main => container.Resolve<MainViewModel>();
    }
}
