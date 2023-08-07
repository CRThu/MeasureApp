using CarrotProtocolLib.Device;
using CarrotProtocolLib.Impl;
using CarrotProtocolLib.Logger;
using CarrotProtocolLib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Protocol
{
    public class AsciiProtocol : IProtocol
    {
        public IDevice Device { get; set; }
        public ILogger Logger { get; set; }

        public AsciiProtocol(IDevice device, ILogger logger)
        {
            Device = device;
            Logger = logger;
        }

        public void Send(IProtocolRecord protocol)
        {
            Send(protocol.Bytes);
        }

        public void Send(byte[] bytes)
        {
            Send(bytes, 0, bytes.Length);
        }

        public void Send(byte[] bytes, int offset, int length)
        {
            Device.Write(bytes, offset, length);
            Logger.AddTx(new AsciiProtocolRecord(bytes, offset, length));
        }
    }
}
