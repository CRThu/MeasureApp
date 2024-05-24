using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Streams
{
    public enum StreamStatus
    {
        Initial,
        Close,
        Open
    }

    public interface IStream
    {
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
}
