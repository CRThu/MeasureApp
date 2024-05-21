using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipelines;
using IOStreamDemo.Streams;
using IOStreamDemo.Protocols;

namespace IOStreamDemo
{
    public class ProtocolRecvService
    {
        public Pipe Pipe { get; set; }

        public ProtocolRecvService()
        {
            Pipe = new Pipe();
        }

        public void Run(IAsyncStream stream, IProtocol protocol)
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            Task wr = FillPipeASync(stream, Pipe.Writer, cts.Token);
            Task rd = ReadPipeASync(protocol, Pipe.Reader, cts.Token);

            Console.WriteLine("RECV...");
            Console.WriteLine("PRESS ANY KEY TO EXIT");
            Console.ReadKey();
            cts.Cancel();
            Console.WriteLine("COMPLETE REQUEST...");

            while (true)
            {
                if (wr.IsCompleted && rd.IsCompleted)
                    break;
            }

            Console.WriteLine("EXIT.");
        }

        // Read From IDriverCommStream and write to pipewriter
        public async Task FillPipeASync(IAsyncStream stream, PipeWriter writer, CancellationToken token)
        {
            const int BUFSIZE = 1048576;
            // TODO:临时缓冲区 后续重构
            byte[] rxTemp = new byte[BUFSIZE];

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                // PipeWriter获取BUFSIZE大小的内存
                Memory<byte> buffer = writer.GetMemory(BUFSIZE);
                try
                {
                    // 读取数据
                    int bytesRead = await stream.ReadAsync(rxTemp, 0, BUFSIZE, token);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    // TODO
                    rxTemp.CopyTo(buffer);
                    // 通知PipeWriter多少数据已写入缓冲区
                    writer.Advance(bytesRead);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    break;
                }

                // Flush数据到PipeReader
                FlushResult result = await writer.FlushAsync(token);

                // 来自PipeReader的EOF处理
                if (result.IsCompleted)
                {
                    await writer.CompleteAsync();
                    break;
                }
            }

            // PipeWriter EOF数据传输结束指示
            await writer.CompleteAsync();
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
                        Console.WriteLine($"RECV PACKET: {packet.Message}");
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