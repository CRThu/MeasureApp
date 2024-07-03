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
    public class RawAsciiProtocolPacket : Packet, IMessagePacket
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

        public RawAsciiProtocolPacket(string msg)
        {
            Encode(msg);
        }

        public byte[] Encode(string msg)
        {
            byte[] packets = (msg + "\r\n").AsciiToBytes();

            return packets;
        }

        public string Decode(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
