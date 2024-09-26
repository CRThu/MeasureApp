using CarrotCommFramework.Util;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data;
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

        public RawAsciiProtocol()
        {
        }

        public static bool TryParse(ref ReadOnlySequence<byte> buffer, out Packet? packet)
        {
            packet = default;

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


            // 处理数据流直到不完整包或结束
            if (!reader.TryPeek(out byte b))
                return false;
            if (b == '<')
            {
                // XML TAG COMMAND
                if (reader.Remaining < xmlCmdTagHeadOpenCompare.Length)
                    return false;
                if (!reader.IsNext(xmlCmdTagHeadOpenCompare, true))
                    return false;
                if (reader.Remaining < xmlCmdTagBinOpenCompare.Length)
                    return false;
                if (!reader.TryReadTo(out ReadOnlySequence<byte> seqHead, xmlCmdTagBinOpenCompare, true))
                    return false;

                // head generate
                //Console.WriteLine($"Read data head: {BytesEx.BytesToAscii(seqHead.ToArray()).ReplaceLineEndings("\\r\\n")}");
                string xmlString = "<head>" + BytesEx.BytesToAscii(seqHead.ToArray()) + "</head>";
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlString);
                XmlElement root = doc.DocumentElement;
                XmlNode node = root.SelectSingleNode("/head/len");

                int dataLen = Convert.ToInt32(node.InnerText);

                if (dataLen < 0)
                    return false;
                if (reader.Remaining < dataLen)
                    return false;
                if (!reader.TryReadExact(dataLen, out ReadOnlySequence<byte> seqBin))
                    return false;

                // packet generate
                //Console.WriteLine($"Read data binary: {BytesEx.BytesToAscii(seqBin.ToArray()).ReplaceLineEndings("\\r\\n")}");

                packet = new BinaryPacket(seqBin.ToArray());

                if (reader.Remaining < xmlCmdTagCloseCompare.Length)
                    return false;
                if (!reader.IsNext(xmlCmdTagCloseCompare, true))
                    return false;

                Console.WriteLine($"Read data head: {BytesEx.BytesToAscii(seqHead.ToArray()).ReplaceLineEndings("\\r\\n")}");
                Console.WriteLine($"Read data binary: {BytesEx.BytesToAscii(seqBin.ToArray()).ReplaceLineEndings("\\r\\n")}");
            }
            else
            {
                // COMMAND END WITH CRLF
                if (reader.Remaining < asciiCmdCrlfCompare.Length)
                    return false;
                if (!reader.TryReadTo(out ReadOnlySequence<byte> seqCmd, asciiCmdCrlfCompare, true))
                    return false;

                // packet generate
                Console.WriteLine($"Read command to CRLF: {BytesEx.BytesToAscii(seqCmd.ToArray()).ReplaceLineEndings("\\r\\n")}");

                packet = new RawAsciiProtocolPacket(seqCmd.ToArray());
            }
            buffer = reader.UnreadSequence;
            return true;
        }
    }
}
