using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrotProtocolLib.Impl;
using CarrotProtocolLib.Util;

namespace CarrotProtocolLib.Interface
{
    public interface IProtocolLog
    {
        public byte[] Bytes { get; }
        public string PayloadDisplay { get; }
    }
}
