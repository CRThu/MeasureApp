using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CarrotProtocolLib.Device;
using CarrotProtocolLib.Logger;
using CarrotProtocolLib.Protocol;

namespace CarrotProtocolLib.Impl
{
    /*
    #define CARROT_DATA_PROTOCOL_GEN(len)                              \
        typedef struct __PROTOCOL_PACKED__                             \
        {                                                              \
            uint8_t frame_start;                                       \
            uint8_t protocol_id;                                       \
            uint16_t control_flags;                                    \
            uint8_t stream_id;                                         \
            uint16_t payload_len;                                      \
            uint8_t payload[len - CARROT_PROTOCOL_DATA_PKG_BYTES];     \
            uint16_t crc16;                                            \
            uint8_t frame_end;                                         \
        }
    carrot_data_protocol_##len;                                  \
    */

    public class CarrotDataProtocol : IProtocol
    {
        public IDevice Device { get; set; }
        public ILogger Logger { get; set; }


        //public delegate void ProtocolParseErrorHandler(Exception ex);
        //public event ProtocolParseErrorHandler ProtocolParseError;
        public event IProtocol.ProtocolParseErrorHandler ProtocolParseError;

        public CarrotDataProtocol(IDevice device, ILogger logger)
        {
            Device = device;
            Logger = logger;
        }

        public CarrotDataProtocol(IDevice device, ILogger logger, IProtocol.ProtocolParseErrorHandler protocolParseErrorHandler) : this(device, logger)
        {
            ProtocolParseError += protocolParseErrorHandler;
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
            Logger.AddTx(new CarrotDataProtocolRecord(bytes, offset, length));
        }

    }
}