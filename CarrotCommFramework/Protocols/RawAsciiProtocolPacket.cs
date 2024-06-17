using CarrotCommFramework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Protocols
{
    /// <summary>
    /// 数据包
    /// </summary>
    public class RawAsciiProtocolPacket : Packet
    {
        /// <summary>
        /// 数据包可阅读信息
        /// </summary>
        public override string? Message => Encoding.ASCII.GetString(Bytes).TrimEnd("\r\n".ToCharArray());

        public override byte? ProtocolId => null;
        public override byte? StreamId => null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public RawAsciiProtocolPacket(byte[] bytes) : base(bytes)
        {
        }

        public RawAsciiProtocolPacket(string? message, byte? protocolId, byte? streamId)
            : base(message, protocolId, streamId)
        {
        }

        public override byte[] Pack(string? message, byte? protocolId, byte? streamId)
        {
            byte[] packets = [.. message.AsciiToBytes(), .. "\r\n".AsciiToBytes()];

            return packets;
        }
    }
}
