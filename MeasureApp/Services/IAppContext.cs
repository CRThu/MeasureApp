using MeasureApp.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Services
{
    public interface IAppContext
    {
        DeviceManager Devices { get; }
        ConfigManager Configs { get; }
    }
}
