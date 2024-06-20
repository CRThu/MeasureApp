using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Streams
{
    public enum StreamStatus
    {
        Initial,
        Close,
        Open
    }

    public interface IStream
    {
        public string Name { get; set; }
        public bool ReadAvailable { get; }

        public Pipe Pipe { get; }

        /// <summary>
        /// 配置解析和初始化
        /// </summary>
        /// <param name="params"></param>
        public void Config(string[] @params = default!);

        /// <summary>
        /// 打开流
        /// </summary>
        /// <param name="addr"></param>
        public abstract void Open();

        /// <summary>
        /// 关闭流
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// 写入字节数组
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public abstract void Write(byte[] buffer, int offset, int count);

        /// <summary>
        /// 读取字节数组
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="bytesExpected"></param>
        /// <returns>返回实际读取字节数</returns>
        public int Read(byte[] buffer, int offset, int count);

    }
    public interface IAsyncStream : IStream
    {
        /// <summary>
        /// 异步读取流
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);
    }

    public abstract class StreamBase : IStream, IAsyncStream
    {
        public string Address { get; set; }
        public string Name { get; set; }

        public virtual bool ReadAvailable { get; }

        public Pipe Pipe { get; set; } = new();


        public abstract void Config(string[] @params = null);

        public abstract void Open();

        public abstract void Close();

        public abstract int Read(byte[] buffer, int offset, int count);

        public abstract void Write(byte[] buffer, int offset, int count);

        /// <summary>
        /// 异步流读取
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            //return ReadAsync(buffer, offset, count, HighPrecisionTimer.HighPrecisionTimer.Delay, 1, cancellationToken);
            return ReadAsync(buffer, offset, count, Task.Delay, 10, cancellationToken);
            //return ReadAsync(buffer, offset, count, (int delay) => { return Task.Run(() => { Thread.Sleep(delay); }); }, 5, cancellationToken);
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
        public virtual Task<int> ReadAsync(byte[] buffer, int offset, int count, Func<int, Task> delayTask, int delayTime, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (ReadAvailable)
                        return Read(buffer, offset, count);
                    else
                        await delayTask(delayTime);
                    //await HighPrecisionTimer.HighPrecisionTimer.Delay();
                }
                return 0;
            }, cancellationToken);
        }

    }
}
