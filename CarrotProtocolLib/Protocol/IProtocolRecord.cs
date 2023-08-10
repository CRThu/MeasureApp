using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrotProtocolLib.Impl;
using CarrotProtocolLib.Util;

namespace CarrotProtocolLib.Protocol
{
    public interface IProtocolRecord
    {
        public byte[] FrameBytes { get; }
        public string PayloadDisplay { get; }
    }
}
