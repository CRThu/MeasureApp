using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Protocols
{
    public class CarrotDataProtocol : ProtocolBase
    {
        public static new string Version { get; } = "CDPV1";

        public string Name { get; set; }

        public CarrotDataProtocol(string name)
        {
            Name = name;
        }

        // TODO
        public override bool TryParse(ref ReadOnlySequence<byte> buffer, out IEnumerable<Packet>? packets)
        {
            List<Packet> packetsList = new();

            // 处理数据流直到不完整包或结束
            while (true)
            {
                SequencePosition? pos = buffer.PositionOf((byte)'\n');

                // 不完整包结构则结束
                if (!pos.HasValue)
                {
                    break;
                }

                Packet pkt = new(buffer.Slice(buffer.Start, pos.Value).ToArray());
                buffer = buffer.Slice(buffer.GetPosition(1, pos.Value));
                packetsList.Add(pkt);
            }

            packets = packetsList;
            return packetsList.Count != 0;
        }
    }
}
