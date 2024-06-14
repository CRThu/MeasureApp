using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CarrotCommFramework.Util.BytesEx;

namespace CarrotCommFramework.Protocols
{
    public class CarrotDataProtocolPacket : Packet
    {
        /// <summary>
        /// 数据包可阅读信息
        /// </summary>
        public new string? Message => Payload.ToArray().BytesToHexString();

        public CarrotDataProtocolPacket(byte[] bytes) : base(bytes) { }


        // TODO
        /// 以下为实现

        /// <summary>
        /// protocol layout index : [0:0]
        /// </summary>
        public byte FrameStart => Bytes[0];
        /// <summary>
        /// protocol layout index : [1:1]
        /// </summary>
        public byte ProtocolId => Bytes[1];
        /// <summary>
        /// protocol layout index : [2:3]
        /// </summary>
        public ushort ControlFlags => (ushort)(Bytes[3] << 8 | Bytes[2]);
        /// <summary>
        /// protocol layout index : [4:4]
        /// </summary>
        public byte StreamId => Bytes[4];
        /// <summary>
        /// protocol layout index : [5:6]
        /// </summary>
        public ushort PayloadLength => (ushort)(Bytes[6] << 8 | Bytes[5]);
        /// <summary>
        /// protocol layout index : [7:6+len]
        /// </summary>
        public ReadOnlySpan<byte> Payload => Bytes.AsSpan(7, PayloadLength);
        /// <summary>
        /// CRC16/MODBUS
        /// protocol layout index : [7+len:8+len]
        /// </summary>
        public ushort Crc16 => (ushort)(Bytes[^2] << 8 | Bytes[^3]);
        /// <summary>
        /// protocol layout index : [9+len:9+len]
        /// </summary>
        public byte FrameEnd => Bytes[^1];


    }

}
