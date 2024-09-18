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
            var startTag = "<data>".AsciiToBytes().AsSpan();
            var endTag = "</data>".AsciiToBytes().AsSpan();
            var crlfTag = "\r\n".AsciiToBytes().AsSpan();
            var reader = new SequenceReader<byte>(buffer);
            ReadOnlySequence<byte> seq;
            ReadOnlySequence<byte> seq2;

            // 处理数据流直到不完整包或结束
            while (true)
            {
                if (reader.TryReadTo(out seq, crlfTag, true))
                {
                    var seqRder = new SequenceReader<byte>(seq);
                    if (seqRder.TryReadTo(out seq2, startTag, true))
                    {
                        if (!seq2.IsEmpty)
                        {
                            Console.WriteLine($"Read ERROR:{BytesEx.BytesToAscii(seq2.ToArray())}");
                        }
                        if (seqRder.TryReadTo(out seq2, endTag, true))
                        {
                            Console.WriteLine($"Read To:{BytesEx.BytesToAscii(seq2.ToArray())}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Read To:{BytesEx.BytesToAscii(seq.ToArray())}");
                    }
                }
                else
                {
                    Console.WriteLine($"Cannot find eof element");
                    break;
                }


                //RawAsciiProtocolPacket pkt = new(buffer.Slice(buffer.Start, pos.Value).ToArray());
                //buffer = buffer.Slice(buffer.GetPosition(1, pos.Value));
                //packetsList.Add(pkt);
            }

            packets = packetsList;
            return packetsList.Count != 0;
        }
    }
}
