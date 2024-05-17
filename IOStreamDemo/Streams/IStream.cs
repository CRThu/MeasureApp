using System;
using System.Collections.Generic;
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
    public interface IAsyncStream
    {
        public bool ReadAvailable { get; }

        /// <summary>
        /// 打开流
        /// </summary>
        /// <param name="addr"></param>
        public abstract void Open();

        /// <summary>
        /// 关闭流
        /// </summary>
        public abstract void Close();

        public Task<int> ReadAsync(byte[] buffer, int offset, int count);
    }
}
