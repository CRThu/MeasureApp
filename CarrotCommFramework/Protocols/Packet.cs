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
    public class Packet
    {
        public static Packet Empty => new([]);

        /// <summary>
        /// 字节数组
        /// </summary>
        public virtual byte[]? Bytes { get; set; }

        /// <summary>
        /// 数据包可阅读信息
        /// </summary>
        public virtual string? Message => null;
        public virtual byte? ProtocolId => null;
        public virtual byte? StreamId => null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public Packet(byte[] bytes)
        {
            Bytes = bytes;
        }

        public Packet(byte[] payload, byte? protocolId, byte? streamId)
        {
            Bytes = Pack(payload, protocolId, streamId);
        }

        public virtual byte[] Pack(byte[] payload, byte? protocolId, byte? streamId)
        {
            return [];
        }
    }
}
