using CarrotLink.Core.Logging;
using CarrotLink.Core.Protocols.Models;
using CarrotLink.Core.Utility;
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
    /// <summary>
    /// 寄存器请求结果指令记录器
    /// </summary>
    public class RegisterLogService : IPacketLogger
    {
        public delegate void RegisterPacketHandler(IRegisterPacket packet, string sender);
        public event RegisterPacketHandler OnRegisterUpdate;

        public void Dispose()
        {

        }

        public void HandlePacket(IPacket packet, string sender)
        {
            if (packet is IRegisterPacket regPacket)
            {
                OnRegisterUpdate?.Invoke(regPacket, sender);
            }
        }
    }
}
