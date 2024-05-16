using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Protocols
{
    public interface IProtocol
    {
        public static string Version { get; }
        public bool TryParse(ref ReadOnlySequence<byte> buffer, out IEnumerable<Packet> packets);
    }
}
