﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipelines;
using CarrotCommFramework.Streams;
using CarrotCommFramework.Protocols;
using System.IO;
using System.Diagnostics;

namespace CarrotCommFramework.Services
{
    public class DataRecvService : SessionServiceBase
    {
        public Task Task { get; set; }

        public DataRecvService()
        {
        }

        // Read From IDriverCommStream and write to pipewriter
        public override async Task Impl()
        {
            PipeWriter writer = Stream!.Pipe.Writer;

            const int BUFSIZE = 1048576;
            // TODO:临时缓冲区
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
                    // TODO 重构ReadAsync
                    //int bytesRead = await Stream!.ReadAsync(buffer, 0, BUFSIZE, Cts.Token);
                    // TODO 性能优化测试
                    int bytesRead = await Stream!.ReadAsync(rxTemp, 0, BUFSIZE, Cts.Token).ConfigureAwait(false);
                    //int bytesRead = await Stream!.ReadAsync(rxTemp, 0, BUFSIZE, Cts.Token);
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
                    Debug.WriteLine(ex.ToString());
                    throw;
                }

                // Flush数据到PipeReader
                // TODO 性能优化测试
                FlushResult result = await writer.FlushAsync(Cts.Token).ConfigureAwait(false);
                //FlushResult result = await writer.FlushAsync(Cts.Token);

                // 来自PipeReader的EOF处理
                if (result.IsCompleted)
                {
                    await writer.CompleteAsync();
                    Debug.WriteLine($"{nameof(DataRecvService)} returning");
                    break;
                }
            }

            // PipeWriter EOF数据传输结束指示
            await writer.CompleteAsync();
            Debug.WriteLine($"{nameof(DataRecvService)} returning");

            Status = ServiceStatus.ExitSuccess;
        }
    }
}