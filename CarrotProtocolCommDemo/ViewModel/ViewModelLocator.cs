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
            container.Register<SessionConfigViewModel>(Reuse.Singleton);
            container.Register<ScriptViewModel>(Reuse.Singleton);

        }

        public MainViewModel? MainVM => container.Resolve<MainViewModel>();
        public SessionConfigViewModel? SessionConfigVM => container.Resolve<SessionConfigViewModel>();
        public ScriptViewModel? ScriptVM => container.Resolve<ScriptViewModel>();
    }
}
