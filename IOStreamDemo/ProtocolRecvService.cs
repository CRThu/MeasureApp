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

        public void Run(IDriverCommStream socket, IProtocol protocol)
        {
            Task wr = FillPipeASync(socket, Pipe.Writer);
            Task rd = ReadPipeASync(protocol, Pipe.Reader);

            Task task = Task.WhenAll(wr, rd);
            task.Wait();
        }

        // Read From IDriverCommStream and write to pipewriter
        public async Task FillPipeASync(IDriverCommStream stream, PipeWriter writer)
        {
            const int BUFSIZE = 1048576;
            // TODO:临时缓冲区 后续重构
            byte[] rxTemp = new byte[BUFSIZE];

            while (true)
            {
                // PipeWriter获取BUFSIZE大小的内存
                Memory<byte> buffer = writer.GetMemory(BUFSIZE);
                try
                {
                    // 读取数据
                    int bytesRead = stream.Read(rxTemp, 0, BUFSIZE);
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
                FlushResult result = await writer.FlushAsync();

                // 来自PiprReader的EOF处理
                if (result.IsCompleted)
                {
                    break;
                }
            }

            // PipeWriter EOF数据传输结束指示
            await writer.CompleteAsync();
        }

        // PipeReader Read From PipeWriter and Process data
        private async Task ReadPipeASync(IProtocol protocol, PipeReader reader)
        {
            while (true)
            {
                // 读取数据, 返回buffer和是否EOF
                ReadResult result = await reader.ReadAsync();
                ReadOnlySequence<byte> buffer = result.Buffer;

                while (protocol.TryParse(ref buffer, out IEnumerable<Packet> line))
                {
                    // 处理数据流
                    Console.WriteLine(line);
                }

                // 通知PipeWriter已读取字节流长度
                reader.AdvanceTo(buffer.Start, buffer.End);


                // 来自PiprWriter EOF数据传输结束
                if (result.IsCompleted)
                {
                    break;
                }
            }

            // PipeReader EOF数据传输结束指示
            await reader.CompleteAsync();
        }
    }
}