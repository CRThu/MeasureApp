using CarrotCommFramework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CarrotCommFramework.Util.BytesEx;

namespace CarrotCommFramework.Protocols
{
    public enum CDP_TYPE
    {
        UNKNOWN = 0x00,
        ASCII = 0x30,
        DATA = 0x40,
        REG = 0xA0
    };


    public class CarrotDataProtocolPacket : Packet
    {
        public const byte CDP_PACKET_START_BYTE = 0x3C;
        public const byte CDP_PACKET_END_BYTE = 0x3E;

        public const byte ProtocolIdAsciiTransfer64 = 0x31;
        public const byte ProtocolIdAsciiTransfer256 = 0x32;
        public const byte ProtocolIdAsciiTransfer2048 = 0x33;
        public const byte ProtocolIdDataTransfer74 = 0x41;
        public const byte ProtocolIdDataTransfer266 = 0x42;
        public const byte ProtocolIdDataTransfer2058 = 0x43;
        public const byte ProtocolIdRegisterOper = 0xA0;
        public const byte ProtocolIdRegisterReply = 0xA8;

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
                ProtocolIdRegisterOper => 256,
                ProtocolIdRegisterReply => 256,
                _ => -1,
            };
        }

        public static CDP_TYPE GetCdpType(byte protocolId)
        {
            if (protocolId >= 0x30 && protocolId <= 0x3F)
                return CDP_TYPE.ASCII;
            else if (protocolId >= 0x40 && protocolId <= 0x4F)
                return CDP_TYPE.DATA;
            else if (protocolId >= 0xA0 && protocolId <= 0xAF)
                return CDP_TYPE.REG;
            else
                return CDP_TYPE.UNKNOWN;
        }

        /// <summary>
        /// 字节数组
        /// </summary>
        public override byte[]? Bytes { get; set; }

        /// <summary>
        /// 数据包可阅读信息
        /// </summary>
        public override string? Message => GetDisplayMessage();
        public override byte? ProtocolId => Bytes?[1];
        public override byte? StreamId => Bytes?[4];

        /// <summary>
        /// 构造函数
        /// </summary>
        public CarrotDataProtocolPacket(byte[] bytes) : base(bytes)
        {
        }

        public CarrotDataProtocolPacket(byte[] payload, byte? protocolId, byte? streamId)
            : base(payload, protocolId, streamId)
        {
        }


        public string GetDisplayMessage()
        {
            if (ProtocolId is null)
                return "<NULL>";
            var type = GetCdpType(ProtocolId.Value);
            switch (type)
            {
                case CDP_TYPE.ASCII:
                    return $"{Payload.ToArray().BytesToAscii().ReplaceLineEndings("")}";
                case CDP_TYPE.DATA:
                    return $"<{PayloadLength} Bytes Data>";
                case CDP_TYPE.REG:
                    return $"{Payload.ToArray().BytesToHexString()}";
                default:
                    return $"<UNKNOWN>";
            }
        }

        public override byte[] Pack(byte[] payload, byte? protocolId, byte? streamId)
        {
            int len = GetPacketLength((byte)protocolId);
            byte[] bytes = new byte[len];

            bytes[0] = CDP_PACKET_START_BYTE;
            bytes[1] = (byte)protocolId;
            //bytes[2] = (byte)ControlFlags;
            //bytes[3] = (byte)(ControlFlags >> 8);
            bytes[2] = 0x00;
            bytes[3] = 0x00;
            bytes[4] = (byte)streamId;
            bytes[5] = (byte)payload.Length;
            bytes[6] = (byte)(payload.Length >> 8);
            Array.Copy(payload, 0, bytes, 7, payload.Length);
            //bytes[^3] = (byte)Crc16;
            //bytes[^2] = (byte)(Crc16 >> 8);
            bytes[^3] = 0xFF;
            bytes[^2] = 0xFF;
            bytes[^1] = CDP_PACKET_END_BYTE;

            return bytes;
        }


        // TODO
        /// 以下为实现

        /// <summary>
        /// protocol layout index : [0:0]
        /// </summary>
        public byte? FrameStart => Bytes?[0];
        /// <summary>
        /// protocol layout index : [1:1]
        /// </summary>
        //public override byte ProtocolId => Bytes[1];
        /// <summary>
        /// protocol layout index : [2:3]
        /// </summary>
        public ushort? ControlFlags => Bytes == null ? null : (ushort)(Bytes[3] << 8 | Bytes[2]);
        /// <summary>
        /// protocol layout index : [4:4]
        /// </summary>
        //public override byte StreamId => Bytes[4];
        /// <summary>
        /// protocol layout index : [5:6]
        /// </summary>
        public int? PayloadLength => Bytes == null ? null : (ushort)(Bytes[6] << 8 | Bytes[5]);
        /// <summary>
        /// protocol layout index : [7:6+len]
        /// </summary>
        public ReadOnlySpan<byte> Payload => Bytes == null ? null : Bytes.AsSpan(7, PayloadLength.Value);
        /// <summary>
        /// CRC16/MODBUS
        /// protocol layout index : [7+len:8+len]
        /// </summary>
        public ushort? Crc16 => Bytes == null ? null : (ushort)(Bytes[^2] << 8 | Bytes[^3]);
        /// <summary>
        /// protocol layout index : [9+len:9+len]
        /// </summary>
        public byte? FrameEnd => Bytes?[^1];


    }

}
