using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Protocols
{
    public interface IProtocol
    {
        /// <summary>
        /// 实例唯一名称
        /// </summary>
        public string Name { get; set; }
        public static string Version { get; }
        public bool TryParse(ref ReadOnlySequence<byte> buffer, out IEnumerable<Packet>? packets);
        public Packet Encode(byte[] payload);
    }

    /// <summary>
    /// 协议基类
    /// </summary>
    public class ProtocolBase : IProtocol
    {
        public static string Version { get; set; } = nameof(ProtocolBase);
        public string Name { get; set; }

        public virtual bool TryParse(ref ReadOnlySequence<byte> buffer, out IEnumerable<Packet>? packets)
        {
            packets = null;
            return false;
        }

        public virtual Packet Encode(byte[] payload)
        {
            return Packet.Empty;
        }
    }
}
