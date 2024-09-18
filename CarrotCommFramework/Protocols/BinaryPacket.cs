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
    public class BinaryPacket : Packet
    {
        /// <summary>
        /// 数据包可阅读信息
        /// </summary>
        public override string? Message => "BinaryPacket";

        public override byte? ProtocolId => null;
        public override byte? StreamId => null;

        public Dictionary<string,string> Desc { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BinaryPacket(byte[] bytes) : base(bytes)
        {
        }
    }
}
