using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MeasureApp.ViewModel
{
    public interface IToolViewModel
    {
        public string Title { get; set; }
        public string ContentId { get; set; }
        public bool IsVisible { get; set; }
        public bool CanClose { get; set; }
        public ICommand CloseCommand { get; set; }
    }
}
