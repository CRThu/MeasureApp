using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrotProtocolLib.Protocol;
using CarrotProtocolLib.Util;

namespace CarrotProtocolLib.Protocol
{
    public interface IProtocolFrame
    {
        public byte[] FrameBytes { get; }
        public string PayloadDisplay { get; }
    }
}
