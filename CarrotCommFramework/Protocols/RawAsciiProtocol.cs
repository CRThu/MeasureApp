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
        private enum RawAsciiProtocolParseStatus
        {
            IDLE,
            XML_CMD_START,
            XML_CMD_WAIT_TAG_HEAD_OPEN,
            XML_CMD_WAIT_TAG_BIN_OPEN,
            XML_CMD_WAIT_TAG_BIN_CONTENT,
            XML_CMD_WAIT_TAG_CLOSE,
            XML_CMD_XML_ERROR,
            ASCII_CMD_WAIT_CRLF,
        }

        public new static string Version { get; } = "RAPV1";

        // fsm
        private RawAsciiProtocolParseStatus Status { get; set; } = 0;

        private int dataLen { get; set; } = 0;

        public RawAsciiProtocol()
        {
        }

        public bool TryParse(ref ReadOnlySequence<byte> buffer, out long comsumedLength)
        {
            var dataOpenTag = "<data>";
            var headOpenTag = "<head>";
            var headCloseTag = "</head>";
            var binaryOpenTag = "<binary>";
            var bDataOpenTag = "<![BDATA[";
            var bDataCloseTag = "]]>";
            var binaryCloseTag = "</binary>";
            var dataCloseTag = "</data>";
            var crlf = "\r\n";

            var xmlCmdTagHeadOpenCompare = $"{dataOpenTag}{headOpenTag}".AsciiToBytes().AsSpan();
            var xmlCmdTagBinOpenCompare = $"{headCloseTag}{binaryOpenTag}{bDataOpenTag}".AsciiToBytes().AsSpan();
            var xmlCmdTagCloseCompare = $"{bDataCloseTag}{binaryCloseTag}{dataCloseTag}{crlf}".AsciiToBytes().AsSpan();
            var asciiCmdCrlfCompare = $"{crlf}".AsciiToBytes().AsSpan();


            var reader = new SequenceReader<byte>(buffer);
            ReadOnlySequence<byte> seqCmd;
            ReadOnlySequence<byte> seqHead;
            ReadOnlySequence<byte> seqBin;

            bool isParseEnd = false;


            // 处理数据流直到不完整包或结束
            while (!isParseEnd)
            {

                switch (Status)
                {
                    case RawAsciiProtocolParseStatus.IDLE:
                        if (reader.TryPeek(out byte b))
                        {
                            Status = (b == '<') ?
                                RawAsciiProtocolParseStatus.XML_CMD_WAIT_TAG_HEAD_OPEN
                                : RawAsciiProtocolParseStatus.ASCII_CMD_WAIT_CRLF;
                        }
                        else
                        {
                            isParseEnd = true;
                            break;
                        }
                        break;
                    case RawAsciiProtocolParseStatus.XML_CMD_WAIT_TAG_HEAD_OPEN:
                        if (reader.Remaining >= xmlCmdTagHeadOpenCompare.Length
                            && reader.IsNext(xmlCmdTagHeadOpenCompare, true))
                        {
                            Status = RawAsciiProtocolParseStatus.XML_CMD_WAIT_TAG_BIN_OPEN;
                        }
                        else
                        {
                            isParseEnd = true;
                            break;
                        }
                        break;
                    case RawAsciiProtocolParseStatus.XML_CMD_WAIT_TAG_BIN_OPEN:
                        if (reader.Remaining >= xmlCmdTagBinOpenCompare.Length
                            && reader.TryReadTo(out seqHead, xmlCmdTagBinOpenCompare, true))
                        {
                            Status = RawAsciiProtocolParseStatus.XML_CMD_WAIT_TAG_BIN_CONTENT;


                            // head generate
                            Console.WriteLine($"Read data head: {BytesEx.BytesToAscii(seqHead.ToArray()).ReplaceLineEndings("\\r\\n")}");
                            string xmlString = "<head>" + BytesEx.BytesToAscii(seqHead.ToArray()) + "</head>";
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(xmlString);
                            XmlElement root = doc.DocumentElement;
                            XmlNode node = root.SelectSingleNode("/head/len");

                            dataLen = Convert.ToInt32(node.InnerText);

                        }
                        else
                        {
                            isParseEnd = true;
                            break;
                        }
                        break;
                    case RawAsciiProtocolParseStatus.XML_CMD_WAIT_TAG_BIN_CONTENT:
                        if (reader.Remaining >= dataLen
                            && reader.TryReadExact(dataLen, out seqBin))
                        {
                            Status = RawAsciiProtocolParseStatus.XML_CMD_WAIT_TAG_CLOSE;


                            // packet generate
                            Console.WriteLine($"Read data binary: {BytesEx.BytesToAscii(seqBin.ToArray()).ReplaceLineEndings("\\r\\n")}");

                            BinaryPacket pkt = new(seqBin.ToArray());
                        }
                        else
                        {
                            isParseEnd = true;
                            break;
                        }
                        break;
                    case RawAsciiProtocolParseStatus.XML_CMD_WAIT_TAG_CLOSE:
                        if (reader.Remaining >= xmlCmdTagCloseCompare.Length
                            && reader.IsNext(xmlCmdTagCloseCompare, true))
                        {
                            Status = RawAsciiProtocolParseStatus.IDLE;
                        }
                        else
                        {
                            isParseEnd = true;
                            break;
                        }
                        break;
                    case RawAsciiProtocolParseStatus.ASCII_CMD_WAIT_CRLF:
                        if (reader.Remaining >= asciiCmdCrlfCompare.Length
                            && reader.TryReadTo(out seqCmd, asciiCmdCrlfCompare, true))
                        {
                            Status = RawAsciiProtocolParseStatus.IDLE;

                            // packet generate
                            Console.WriteLine($"Read command to CRLF: {BytesEx.BytesToAscii(seqCmd.ToArray()).ReplaceLineEndings("\\r\\n")}");

                            RawAsciiProtocolPacket pkt = new(seqCmd.ToArray());
                        }
                        else
                        {
                            isParseEnd = true;
                            break;
                        }
                        break;
                }

                if (isParseEnd)
                    break;
            }
            comsumedLength = reader.Consumed;
            buffer = reader.UnreadSequence;
            return comsumedLength != 0;
        }
    }
}
