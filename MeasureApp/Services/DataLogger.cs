using CarrotLink.Core.Logging;
using CarrotLink.Core.Protocols.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MeasureApp.Services
{
    public class DataLogInternal<T>
    {
        private List<double> data = new List<double>();
        public List<double> Data => data;
    }

    public partial class DataLogService : ObservableObject, IPacketLogger
    {
        [ObservableProperty]
        private List<double> logs = new List<double>();

        public void HandlePacket(IPacket packet)
        {

        }

        public void Dispose()
        {

        }

    }
}
