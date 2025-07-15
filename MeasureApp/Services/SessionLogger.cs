using CarrotLink.Core.Logging;
using CarrotLink.Core.Protocols.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Services
{
    public partial class SessionLogger : ObservableObject, IPacketLogger
    {
        public void Dispose()
        {

        }

        public void HandlePacket(IPacket packet)
        {

        }
    }
}
