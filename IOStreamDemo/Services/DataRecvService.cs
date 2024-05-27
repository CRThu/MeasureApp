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
    public class DataRecvService : IService
    {
        public Pipe Pipe { get; set; }

        public CancellationTokenSource Cts { get; set; }

        public IAsyncStream Stream { get; set; }

        public Task Task { get; set; }

        public string Name { get; set; }

        public DataRecvService(string name)
        {
            Name = name;
        }

        public event IService.LogEventHandler Logging;

        public void Start()
        {
            Task = FillPipeASync(Stream, Pipe.Writer, Cts.Token);
        }

        public void Stop()
        {
            Cts.Cancel();
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

        public void Bind(IStream stream, IProtocol protocol)
        {
            Pipe = stream.Pipe;
            Stream = (IAsyncStream)stream;
            Cts = new CancellationTokenSource();
        }
    }
}