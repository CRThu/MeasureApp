using MeasureApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Services
{
    public class LayoutService
    {
        public ObservableCollection<IToolViewModel> Tools { get; } = new ObservableCollection<IToolViewModel>();

        public LayoutService(IEnumerable<IToolViewModel> tools)
        {
            foreach (var tool in tools)
            {
                Tools.Add(tool);
            }
        }
    }
}
