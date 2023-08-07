using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CarrotProtocolLib.Device;
using CarrotProtocolLib.Logger;
using CarrotProtocolLib.Protocol;
using CarrotProtocolLib.Service;

namespace CarrotProtocolLib.Impl
{

    public class CarrotDataProtocol
    {
        public CarrotDataProtocol()
        {
        }

        public static IService GetDecodeService()
        {
            return new CarrotDataProtocolDecodeService();
        }

        public static CarrotDataProtocolRecord Create(int protocolId, int streamId, string payload)
        {
            return new CarrotDataProtocolRecord(protocolId, streamId, payload);
        }

        public static CarrotDataProtocolRecord Create(byte[] bytes, int offset, int length)
        {
            return new CarrotDataProtocolRecord(bytes, offset, length);
        }
    }
}