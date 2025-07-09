using CarrotLink.Core.Services;
using System.Collections.Concurrent;

namespace MeasureApp.Services
{
    public class DeviceManager
    {
        public readonly ConcurrentDictionary<string, DeviceService> _connectedServices = new ConcurrentDictionary<string, DeviceService>();


    }
}