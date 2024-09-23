using CarrotCommFramework.Util;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
            var dataStartTag = "<data>".AsciiToBytes().AsSpan();
            var headStartTag = "<head>".AsciiToBytes().AsSpan();
            var headEndTag = "</head>".AsciiToBytes().AsSpan();
            var binaryStartTag = "<binary><![BDATA[".AsciiToBytes().AsSpan();
            var binaryEndTag = "]]></binary>".AsciiToBytes().AsSpan();
            var dataEndTag = "</data>".AsciiToBytes().AsSpan();
            var crlfTag = "\r\n".AsciiToBytes().AsSpan();
            var reader = new SequenceReader<byte>(buffer);
            ReadOnlySequence<byte> seq;
            int dataLen = 0;

            // 处理数据流直到不完整包或结束
            while (true)
            {
                // detect tag symbol '<data>'
                if (reader.IsNext(dataStartTag))
                {
                    reader.Advance(dataStartTag.Length);
                    // detect tag symbol '<head>'
                    if (reader.IsNext(headStartTag))
                    {
                        reader.Advance(headStartTag.Length);

                        // read to tag symbol '</head>'
                        if (reader.TryReadTo(out seq, headEndTag, true))
                        {
                            Console.WriteLine($"Read data head: {BytesEx.BytesToAscii(seq.ToArray()).ReplaceLineEndings("\\r\\n")}");
                            string xmlString = "<head>" + BytesEx.BytesToAscii(seq.ToArray()) + "</head>";
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(xmlString);
                            XmlElement root = doc.DocumentElement;
                            XmlNode node = root.SelectSingleNode("/head/len");

                            dataLen = Convert.ToInt32(node.InnerText);
                        }

                        // detect tag symbol '<binary><![BDATA['
                        if (reader.IsNext(binaryStartTag))
                        {
                            reader.Advance(binaryStartTag.Length);

                            // read binary bytes
                            if (reader.TryReadExact(dataLen, out seq))
                            {
                                Console.WriteLine($"Read data binary: {BytesEx.BytesToAscii(seq.ToArray()).ReplaceLineEndings("\\r\\n")}");
                            }

                            // detect tag symbol ']]></binary>'
                            if (reader.IsNext(binaryEndTag))
                            {
                                reader.Advance(binaryEndTag.Length);
                            }
                        }
                    }
                    // detect tag symbol '</data>'
                    if (reader.IsNext(dataEndTag))
                    {
                        reader.Advance(dataEndTag.Length);
                    }
                }
                else if (reader.TryReadTo(out seq, crlfTag, true))
                {
                    if (!seq.IsEmpty)
                    {
                        Console.WriteLine($"Read command to CRLF: {BytesEx.BytesToAscii(seq.ToArray()).ReplaceLineEndings("\\r\\n")}");
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
