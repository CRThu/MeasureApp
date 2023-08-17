using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CarrotProtocolLib.Logger;
using CarrotProtocolLib.Util;

namespace CarrotProtocolLib.Protocol
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

    public class CarrotDataProtocolFrame : IProtocolFrame
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
        public string PayloadDisplay => Payload.BytesToEscapeString();

        public const byte FrameStartByte = 0x3C;
        public const byte FrameEndByte = 0x3E;

        public const byte ProtocolIdAsciiTransfer64 = 0x31;
        public const byte ProtocolIdAsciiTransfer256 = 0x32;
        public const byte ProtocolIdAsciiTransfer2048 = 0x33;
        public const byte ProtocolIdDataTransfer74 = 0x41;
        public const byte ProtocolIdDataTransfer266 = 0x42;
        public const byte ProtocolIdDataTransfer2058 = 0x43;

        /// <summary>
        /// byte[]转协议
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <exception cref="NotImplementedException"></exception>
        public CarrotDataProtocolFrame(byte[] bytes, int offset, int length)
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

        public CarrotDataProtocolFrame(int protocolId, int streamId, string payload, bool isCrc = true)
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

        public CarrotDataProtocolFrame(int protocolId, int streamId, byte[] payload, bool isCrc = true)
        {
            byte[] newPayload = payload;
            if (payload.Length < 1 || payload[^1] != '\n')
                newPayload = payload.Concat("\r\n"u8.ToArray()).ToArray();

            // Payload过长时截尾
            if (newPayload.Length + 10 > GetPacketLength((byte)protocolId))
                newPayload = newPayload.Take(GetPacketLength((byte)protocolId) - 10).ToArray();

            FrameStart = 0x3C;
            ProtocolId = (byte)protocolId;
            ControlFlags = 0x0000;
            StreamId = (byte)streamId;
            PayloadLength = (ushort)newPayload.Length;
            Payload = newPayload;
            Crc16 = (ushort)(isCrc ? GenerateCrc() : 0xCCCC);
            FrameEnd = 0x3E;
        }

        /// <summary>
        /// 预设协议长度
        /// </summary>
        /// <param name="ProtocolId"></param>
        /// <returns></returns>
        public static int GetPacketLength(byte protocolId)
        {
            return protocolId switch
            {

                ProtocolIdAsciiTransfer64 => 64,
                ProtocolIdAsciiTransfer256 => 256,
                ProtocolIdAsciiTransfer2048 => 2048,
                ProtocolIdDataTransfer74 => 64 + 10,
                ProtocolIdDataTransfer266 => 256 + 10,
                ProtocolIdDataTransfer2058 => 2048 + 10,
                _ => -1,
            };
        }

        public static TransferType GetTransferType(byte protocolId)
        {
            return protocolId switch
            {
                ProtocolIdAsciiTransfer64 => TransferType.Command,
                ProtocolIdAsciiTransfer256 => TransferType.Command,
                ProtocolIdAsciiTransfer2048 => TransferType.Command,
                ProtocolIdDataTransfer74 => TransferType.Data,
                ProtocolIdDataTransfer266 => TransferType.Data,
                ProtocolIdDataTransfer2058 => TransferType.Data,
                _ => TransferType.Command
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

        public IRecord ToRecord(string from, string to)
        {
            return new ProtocolRecord()
            {
                Time = DateTime.Now,
                From = from,
                To = to,
                Stream = StreamId,
                ProtocolName = nameof(CarrotDataProtocolFrame),
                Type = GetTransferType(ProtocolId),
                Frame = this
            };
        }

        /// <summary>
        /// 解析为数据
        /// </summary>
        /// <returns></returns>
        public double[] DecodeData()
        {
            if (GetTransferType(ProtocolId) == TransferType.Data)
            {
                int cnt = PayloadLength / 4;
                double[] numArray = new double[cnt];
                for (int i = 0; i < cnt; i++)
                {
                    numArray[i] = Payload.BytesToInt(i * 4);
                }
                return numArray;
            }
            return Array.Empty<double>();
        }
    }
}
