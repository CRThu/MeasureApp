using IOStreamDemo.Loggers;
using IOStreamDemo.Protocols;
using IOStreamDemo.Streams;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Services
{
    public class ProtocolParseService : IService
    {
        public Pipe Pipe { get; set; }

        public CancellationTokenSource Cts { get; set; }

        public IProtocol Protocol { get; set; }

        public Task Task { get; set; }

        public string Name { get; set; }

        public ProtocolParseService(string name)
        {
            Name = name;
        }

        public event IService.LogEventHandler Logging;

        public void Start()
        {
            Task = ReadPipeASync(Protocol, Pipe.Reader, Cts.Token);
        }

        public void Stop()
        {
            Cts.Cancel();
        }

        // PipeReader Read From PipeWriter and Process data
        private async Task ReadPipeASync(IProtocol protocol, PipeReader reader, CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                // 读取数据, 返回buffer和是否EOF
                ReadResult result = await reader.ReadAsync(token);
                ReadOnlySequence<byte> buffer = result.Buffer;

                while (protocol.TryParse(ref buffer, out IEnumerable<Packet> pkts))
                {
                    // 处理数据流
                    foreach (Packet packet in pkts)
                    {
                        //Console.WriteLine($"RECV PACKET: {packet.Message}");
                        Logging?.Invoke(this, new LogEventArgs() { Packet = packet });
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

        public void Bind(IStream stream, IProtocol protocol)
        {
            Pipe = stream.Pipe;
            Protocol = protocol;
            Cts = new CancellationTokenSource();
        }
    }
}
