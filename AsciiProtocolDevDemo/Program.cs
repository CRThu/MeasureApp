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
            var crlf = "\r\n".AsSpan();
            var dataTagStart = "<data>".AsSpan();
            var dataTagEnd = "</data>".AsSpan();

            string rdStr = "COMMAND1_PLACEHOLDER\r\n<data>DATA_PLACEHOLDER</data>\r\nCOMMAND2_PLACEHOLDER\r\n";
            Console.WriteLine($"BUF:{rdStr.ReplaceLineEndings("\\r\\n")}");
            var rdSeq = new ReadOnlySequence<byte>(Encoding.ASCII.GetBytes(rdStr.ToCharArray()));
            RawAsciiProtocol asciiProtocol = new();
            bool ret = asciiProtocol.TryParse(ref rdSeq, out var pkts);
            if (ret)
            {
                foreach (var pkt in pkts)
                {
                    Console.WriteLine($"PKTMSG:{pkt.Message}");
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
