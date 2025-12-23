using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.Plugins.Interfaces
{
    public interface IMeasureAppPlugin
    {
        public string Name { get; }
        public FrameworkElement View { get; }
    }
}
