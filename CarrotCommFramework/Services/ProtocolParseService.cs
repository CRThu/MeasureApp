﻿using CarrotCommFramework.Loggers;
using CarrotCommFramework.Protocols;
using CarrotCommFramework.Streams;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
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
            try
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

                    bool isPacketsResolved = Protocol!.TryParse(ref buffer, out IEnumerable<Packet>? ResolvedPackets, out long comsumedLength);
                    if (isPacketsResolved)
                    {
                        // 处理数据流
                        foreach (Packet packet in ResolvedPackets)
                        {
                            //Debug.WriteLine($"RECV PACKET: {packet.Message}");
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
                    reader.AdvanceTo(buffer.GetPosition(comsumedLength));


                    // 来自PipeWriter EOF数据传输结束
                    if (result.IsCompleted)
                    {
                        await reader.CompleteAsync();
                        Debug.WriteLine($"{nameof(ProtocolParseService)} returning");
                        break;
                    }
                }

                // PipeReader EOF数据传输结束指示
                await reader.CompleteAsync();
                Debug.WriteLine($"{nameof(ProtocolParseService)} returning");

                Status = ServiceStatus.ExitSuccess;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                throw;
            }
        }
    }
}
