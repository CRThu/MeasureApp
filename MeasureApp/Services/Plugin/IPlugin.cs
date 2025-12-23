using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Services.Plugin
{
    public interface IPlugin
    {
        public string Title { get; }
        object View { get; }
    }
}
