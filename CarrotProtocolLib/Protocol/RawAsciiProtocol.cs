using CarrotProtocolLib.Device;
using CarrotProtocolLib.Impl;
using CarrotProtocolLib.Logger;
using CarrotProtocolLib.Service;
using CarrotProtocolLib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Protocol
{
    public class RawAsciiProtocol
    {
        public RawAsciiProtocol()
        {
        }

        public static IService GetDecodeService()
        {
            return new RawAsciiProtocolDecodeService();
        }

        public static RawAsciiProtocolRecord Create(string payload)
        {
            return new RawAsciiProtocolRecord(payload);
        }

        public static RawAsciiProtocolRecord Create(byte[] bytes, int offset, int length)
        {
            return new RawAsciiProtocolRecord(bytes, offset, length);
        }
    }
}
