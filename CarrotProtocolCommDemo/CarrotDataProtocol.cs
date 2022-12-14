using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolCommDemo
{
    public class CarrotDataProtocol
    {
        /// <summary>
        /// protocol layout index : [0:0]
        /// </summary>
        public byte FrameStart { get; set; }
        /// <summary>
        /// protocol layout index : [1:1]
        /// </summary>
        public byte ProtocolId { get; set; }
        /// <summary>
        /// protocol layout index : [2:3]
        /// </summary>
        public ushort ControlFlags { get; set; }
        /// <summary>
        /// protocol layout index : [4:4]
        /// </summary>
        public byte StreamId { get; set; }
        /// <summary>
        /// protocol layout index : [5:6]
        /// </summary>
        public ushort PayloadLength { get; set; }
        /// <summary>
        /// protocol layout index : [7:6+len]
        /// </summary>
        public byte[] Payload { get; set; }
        /// <summary>
        /// protocol layout index : [7+len:8+len]
        /// </summary>
        public ushort Crc16 { get; set; }
        /// <summary>
        /// protocol layout index : [9+len:9+len]
        /// </summary>
        public byte FrameEnd { get; set; }

        public CarrotDataProtocol()
        {

        }

        public CarrotDataProtocol(byte[] bytes, int offset, int length)
        {
            FrameStart = bytes[offset + 0];
            ProtocolId = bytes[offset + 1];
            ControlFlags = (ushort)(bytes[offset + 3] << 8 | bytes[offset + 2]);
            StreamId = bytes[offset + 4];
            PayloadLength = (ushort)(bytes[offset + 6] << 8 | bytes[offset + 5]);
            Payload = new byte[PayloadLength];
            Array.Fill<byte>(Payload, 0);
            Array.Copy(bytes, offset + 7, Payload, 0, PayloadLength);
            Crc16 = (ushort)(bytes[offset + length - 2] << 8 | bytes[offset + length - 3]);
            FrameEnd = bytes[offset + length - 1];

            // 传入长度与协议定义不一致
            if (GetPacketLength(ProtocolId) != length)
                throw new NotImplementedException();
        }

        public static int GetPacketLength(byte ProtocolId)
        {
            return ProtocolId switch
            {
                0x30 => 16,
                0x31 => 256,
                0x32 => 2048,
                _ => -1,
            };
        }

        public override string ToString()
        {
            return $"{{ FrameStart: 0x{FrameStart:X2}, " +
                $"ProtocolId: 0x{ProtocolId:X2}, " +
                $"ControlFlags: 0x{ControlFlags:X4}, " +
                $"StreamId: 0x{StreamId:X2}, " +
                $"PayloadLength: 0x{PayloadLength:X4}, " +
                $"Payload: 0x{BytesEx.BytesToHexString(Payload)}, " +
                $"Crc16: 0x{Crc16:X4}, " +
                $"FrameEnd: 0x{FrameEnd:X2} }}";
        }
    }
}
