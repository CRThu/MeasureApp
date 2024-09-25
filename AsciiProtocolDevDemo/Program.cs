using CarrotCommFramework.Protocols;
using System.Buffers;
using System.Reflection.Metadata;
using System.Text;

namespace AsciiProtocolDevDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string binData = "{{DATA.BIN.PLACEHOLDER.1}}" +
                "{{\r\n</bin>\r\n</data>\r\n}}" +
                "{{DATA.BIN.PLACEHOLDER.2}}";
            string rdStr = "COMMAND.PLACEHOLDER.1\r\n" +
                "<data>" +
                "<head>" +
                    "<desc>DATA.HEAD.PLACEHOLDER</desc>" +
                    $"<len>{binData.Length}</len>" +
                "</head>" +
                "<binary>" +
                    "<![BDATA[" +
                        binData +
                    "]]>" +
                "</binary>" +
                "</data>\r\n" +
                "COMMAND.PLACEHOLDER.2\r\n";
            Console.WriteLine($"BUF:{rdStr.ReplaceLineEndings("\\r\\n")}");
            var rdSeq = new ReadOnlySequence<byte>(Encoding.ASCII.GetBytes(rdStr.ToCharArray()));
            RawAsciiProtocol asciiProtocol = new();
            bool ret = asciiProtocol.TryParse(ref rdSeq, out var pkts);
            if (ret)
            {
                foreach (var pkt in pkts)
                {
                    Console.WriteLine($"PKTMSG: {pkt.Message}");
                }
            }
            else
            {
                Console.WriteLine("NO PACKETS CAN BE PARSED");
            }
            return;

            /*
            while (true)
            {
                string rd = Console.ReadLine();
                var rdSeq = new ReadOnlySequence<char>(rd.ToCharArray());
                var rdSeqR = new SequenceReader<char>(rdSeq);
                if (rdSeqR.TryReadTo(out ReadOnlySequence<char> seq, dataTagStart, true))
                {
                    Console.WriteLine($"Read To:{new string(seq.ToArray())}");
                    if (rdSeqR.TryReadTo(out seq, dataTagEnd, true))
                    {
                        Console.WriteLine($"Read To:{new string(seq.ToArray())}");
                    }
                }
                else if (rdSeqR.TryReadTo(out seq, crlf, true))
                {
                    Console.WriteLine($"Read To:{new string(seq.ToArray())}");
                    Console.WriteLine($"Unread:{new string(rdSeqR.UnreadSequence.ToArray())}");
                }
                else
                    Console.WriteLine($"Cannot find eof element");
            }

            */
            Console.WriteLine("Hello, World!");
        }
    }
}
