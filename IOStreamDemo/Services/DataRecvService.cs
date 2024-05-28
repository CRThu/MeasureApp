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
using System.IO;

namespace IOStreamDemo.Services
{
    public class DataRecvService : SessionServiceBase
    {
        public Task Task { get; set; }

        public string Name { get; set; }

        public DataRecvService(string name)
        {
            Name = name;
        }

        // Read From IDriverCommStream and write to pipewriter
        public override async Task Impl()
        {
            PipeWriter writer = Stream!.Pipe.Writer;

            const int BUFSIZE = 1048576;
            // TODO:临时缓冲区 后续重构
            byte[] rxTemp = new byte[BUFSIZE];

            while (true)
            {
                if (Cts.Token.IsCancellationRequested)
                {
                    break;
                }

                // PipeWriter获取BUFSIZE大小的内存
                Memory<byte> buffer = writer.GetMemory(BUFSIZE);
                try
                {
                    // 读取数据
                    int bytesRead = await Stream!.ReadAsync(rxTemp, 0, BUFSIZE, Cts.Token);
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
                FlushResult result = await writer.FlushAsync(Cts.Token);

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
    }
}