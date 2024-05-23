using CarrotProtocolLib.Device;
using HighPrecisionTimer;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Streams
{
    public class SerialStream : IStream, IAsyncStream
    {
        public string Address { get; set; }
        public string Name { get; set; }

        public SerialStream(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 流指示有数据
        /// </summary>
        public bool ReadAvailable => Driver.BytesToRead > 0;

        /// <summary>
        /// 驱动层实现
        /// </summary>
        private SerialPort Driver { get; set; } = new();

        /// <summary>
        /// 配置解析和初始化
        /// </summary>
        /// <param name="cfg"></param>
        public void Config(string? cfg)
        {
            if (cfg == null)
                return;

            Driver = new SerialPort(cfg);
        }

        /// <summary>
        /// 关闭流
        /// </summary>
        public void Close()
        {
            Driver.Close();
        }

        /// <summary>
        /// 打开流
        /// </summary>
        public void Open()
        {
            Driver.Open();
        }

        /// <summary>
        /// 流写入
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void Write(byte[] buffer, int offset, int count)
        {
            Driver.BaseStream.Write(buffer, offset, count);
        }

        /// <summary>
        /// 流读取
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            // TODO 同步流读取存在阻塞，待优化
            return Driver.BaseStream.Read(buffer, offset, count);
        }

        /// <summary>
        /// 异步流读取
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return ReadAsync(buffer, offset, count, cancellationToken, HighPrecisionTimer.HighPrecisionTimer.Delay);
            //return ReadAsync(buffer, offset, count, cancellationToken, Task.Delay);
            //return ReadAsync(buffer, offset, count, cancellationToken, (int delay) => { return Task.Run(() => { Thread.Sleep(delay); }); });
        }

        /// <summary>
        /// 异步流读取
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="delayTask"></param>
        /// <returns></returns>
        public Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken, Func<int, Task> delayTask)
        {
            return Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (ReadAvailable)
                        return Read(buffer, offset, count);
                    else
                        await delayTask(5);
                    //await HighPrecisionTimer.HighPrecisionTimer.Delay();
                }
                return 0;
            }, cancellationToken);
        }
    }
}
