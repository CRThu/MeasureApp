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
        public static string Version { get; }
        public bool TryParse(ref ReadOnlySequence<byte> buffer, out IEnumerable<Packet>? packets);
    }

    /// <summary>
    /// 协议基类
    /// </summary>
    public class ProtocolBase : IProtocol
    {
        public static string Version { get; set; } = nameof(ProtocolBase);

        public virtual bool TryParse(ref ReadOnlySequence<byte> buffer, out IEnumerable<Packet>? packets)
        {
            packets = null;
            return false;
        }
    }
}
