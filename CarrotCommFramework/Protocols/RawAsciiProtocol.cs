using CarrotCommFramework.Util;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Protocols
{
    public class RawAsciiProtocol : ProtocolBase
    {
        public new static string Version { get; } = "RAPV1";

        public RawAsciiProtocol()
        {
        }

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

                RawAsciiProtocolPacket pkt = new(buffer.Slice(buffer.Start, pos.Value).ToArray());
                buffer = buffer.Slice(buffer.GetPosition(1, pos.Value));
                packetsList.Add(pkt);
            }

            packets = packetsList;
            return packetsList.Count != 0;
        }

        public override Packet Encode(string? message, byte? protocolId, byte? streamId)
        {
            return new RawAsciiProtocolPacket(message, protocolId, streamId);
        }
    }
}
