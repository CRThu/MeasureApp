using CarrotCommFramework.Loggers;
using CarrotCommFramework.Protocols;
using CarrotCommFramework.Streams;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Services
{
    public class ProtocolParseService : SessionServiceBase
    {
        public Task Task { get; set; }

        public string Name { get; set; }

        public ProtocolParseService()
        {
        }

        // PipeReader Read From PipeWriter and Process data
        public override async Task Impl()
        {
            PipeReader reader = Stream!.Pipe.Reader;

            while (true)
            {
                if (Cts.Token.IsCancellationRequested)
                {
                    break;
                }

                // 读取数据, 返回buffer和是否EOF
                ReadResult result = await reader.ReadAsync(Cts.Token);
                ReadOnlySequence<byte> buffer = result.Buffer;

                while (Protocol!.TryParse(ref buffer, out IEnumerable<Packet> pkts))
                {
                    // 处理数据流
                    foreach (Packet packet in pkts)
                    {
                        //Console.WriteLine($"RECV PACKET: {packet.Message}");
                        OnServiceLogging(this,
                            new LogEventArgs()
                            {
                                Time = DateTime.Now,
                                From = "RX",
                                Packet = packet
                            });
                    }
                }

                // 通知PipeWriter已读取字节流长度
                reader.AdvanceTo(buffer.Start, buffer.End);


                // 来自PiprWriter EOF数据传输结束
                if (result.IsCompleted)
                {
                    await reader.CompleteAsync();
                    break;
                }
            }

            // PipeReader EOF数据传输结束指示
            await reader.CompleteAsync();
        }
    }
}
