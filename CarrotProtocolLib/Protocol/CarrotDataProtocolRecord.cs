using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CarrotProtocolLib.Protocol;
using CarrotProtocolLib.Util;

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

    public class CarrotDataProtocolRecord : IProtocolRecord
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
        /// CRC16/MODBUS
        /// protocol layout index : [7+len:8+len]
        /// </summary>
        public ushort Crc16 { get; set; }
        /// <summary>
        /// protocol layout index : [9+len:9+len]
        /// </summary>
        public byte FrameEnd { get; set; }


        public byte[] FrameBytes => ToBytes();
        public string PayloadDisplay => BytesEx.BytesToAscii(Payload);

        public static byte FrameStartByte = 0x3C;
        public static byte FrameEndByte = 0x3E;

        /// <summary>
        /// byte[]转协议
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <exception cref="NotImplementedException"></exception>
        public CarrotDataProtocolRecord(byte[] bytes, int offset, int length)
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

        public CarrotDataProtocolRecord(int protocolId, int streamId, string payload, bool isCrc = true)
        {
            if (payload.Length < 2 || payload[^2..^1] != "\r\n")
                payload += "\r\n";

            // Payload过长
            if (payload.Length + 10 > GetPacketLength((byte)protocolId))
                throw new NotImplementedException();

            FrameStart = 0x3C;
            ProtocolId = (byte)protocolId;
            ControlFlags = 0x0000;
            StreamId = (byte)streamId;
            PayloadLength = (ushort)payload.Length;
            Payload = BytesEx.AsciiToBytes(payload);
            Crc16 = (ushort)(isCrc ? GenerateCrc() : 0xCCCC);
            FrameEnd = 0x3E;
        }

        /// <summary>
        /// 预设协议长度
        /// </summary>
        /// <param name="ProtocolId"></param>
        /// <returns></returns>
        public static int GetPacketLength(byte ProtocolId)
        {
            return ProtocolId switch
            {
                0x30 => 16,
                0x31 => 64,
                0x32 => 256,
                0x33 => 2048,
                0x41 => 64 + 10,
                0x42 => 256 + 10,
                0x43 => 2048 + 10,
                _ => -1,
            };
        }

        /// <summary>
        /// CRC16校验生成
        /// https://crccalc.com/
        /// </summary>
        /// <returns></returns>
        public ushort GenerateCrc()
        {
            return NullFX.CRC.Crc16.ComputeChecksum(NullFX.CRC.Crc16Algorithm.Modbus, ToBytes()[1..^3]);
        }

        /// <summary>
        /// CRC16校验比对
        /// </summary>
        /// <returns></returns>
        public bool CheckCrcError()
        {
            return GenerateCrc() != Crc16;
        }

        /// <summary>
        /// 协议转byte[]
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public byte[] ToBytes()
        {
            int len = GetPacketLength(ProtocolId);
            byte[] bytes = new byte[len];

            if (PayloadLength + 10 > bytes.Length)
                throw new NotImplementedException();

            bytes[0] = FrameStart;
            bytes[1] = ProtocolId;
            bytes[2] = (byte)ControlFlags;
            bytes[3] = (byte)(ControlFlags >> 8);
            bytes[4] = StreamId;
            bytes[5] = (byte)PayloadLength;
            bytes[6] = (byte)(PayloadLength >> 8);
            Array.Copy(Payload, 0, bytes, 7, PayloadLength);
            bytes[^3] = (byte)Crc16;
            bytes[^2] = (byte)(Crc16 >> 8);
            bytes[^1] = FrameEnd;

            return bytes;
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
