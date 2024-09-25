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
            ReadOnlySequence<byte> seqCmd;
            ReadOnlySequence<byte> seqHead;
            ReadOnlySequence<byte> seqBin;
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
                        if (reader.TryReadTo(out seqHead, headEndTag, true))
                        {
                            Console.WriteLine($"Read data head: {BytesEx.BytesToAscii(seqHead.ToArray()).ReplaceLineEndings("\\r\\n")}");
                            string xmlString = "<head>" + BytesEx.BytesToAscii(seqHead.ToArray()) + "</head>";
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
                            if (reader.TryReadExact(dataLen, out seqBin))
                            {
                                Console.WriteLine($"Read data binary: {BytesEx.BytesToAscii(seqBin.ToArray()).ReplaceLineEndings("\\r\\n")}");
                            }

                            // detect tag symbol ']]></binary>'
                            if (reader.IsNext(binaryEndTag))
                            {
                                reader.Advance(binaryEndTag.Length);
                            }

                            BinaryPacket pkt = new(seqBin.ToArray());
                            packetsList.Add(pkt);
                        }
                    }
                    // detect tag symbol '</data>'
                    if (reader.IsNext(dataEndTag))
                    {
                        reader.Advance(dataEndTag.Length);
                    }
                }
                else if (reader.TryReadTo(out seqCmd, crlfTag, true))
                {
                    if (!seqCmd.IsEmpty)
                    {
                        Console.WriteLine($"Read command to CRLF: {BytesEx.BytesToAscii(seqCmd.ToArray()).ReplaceLineEndings("\\r\\n")}");
                        
                        RawAsciiProtocolPacket pkt = new(seqCmd.ToArray());
                        packetsList.Add(pkt);
                    }
                }
                else
                {
                    //Console.WriteLine($"Cannot find eof element");
                    break;
                }
            }

            packets = packetsList;
            return packetsList.Count != 0;
        }
    }
}
