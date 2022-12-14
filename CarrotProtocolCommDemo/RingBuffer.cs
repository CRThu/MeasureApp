using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolCommDemo
{
    /// <summary>
    /// FIFO环形缓冲区
    /// </summary>
    public class RingBuffer
    {
        /// <summary>
        /// 缓冲区
        /// </summary>
        public byte[] Buffer { get; set; }
        /// <summary>
        /// 数据字节数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 数据头指针索引
        /// </summary>
        public int HeadIndex { get; set; }
        /// <summary>
        /// 数据尾指针索引
        /// </summary>
        public int TailIndex { get; set; }

        /// <summary>
        /// 缓冲区初始化
        /// </summary>
        /// <param name="bufferSize"></param>
        public RingBuffer(int bufferSize)
        {
            Count = 0;
            HeadIndex = 0;
            TailIndex = 0;
            Buffer = new byte[bufferSize];
        }

        /// <summary>
        /// 获取环形缓冲区下标位置字节
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public byte this[int index]
        {
            get
            {
                if (index >= Count)
                    throw new IndexOutOfRangeException("环形缓冲区索引溢出");
                return Buffer[(HeadIndex + index) % Buffer.Length];
            }
        }

        /// <summary>
        /// 清空缓冲区
        /// </summary>
        public void Clear()
        {
            Count = 0;
            HeadIndex = 0;
            TailIndex = 0;
        }

        /// <summary>
        /// 写缓冲区
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void Write(byte[] buffer, int offset, int count)
        {
            // 缓冲区满
            if (Count + count > Buffer.Length)
            {
                throw new IndexOutOfRangeException("环形缓冲区过小");
            }

            int bytesToWrite = count;
            int bytesCount = (HeadIndex + count < Buffer.Length) ? count : (Buffer.Length - HeadIndex);

            // 缓冲区写入
            Array.Copy(buffer, offset, Buffer, HeadIndex, bytesCount);
            HeadIndex += bytesCount;
            Count += bytesCount;
            bytesToWrite -= bytesCount;

            // 跨缓冲区边界写入
            if (bytesToWrite != 0)
            {
                HeadIndex = 0;
                Array.Copy(buffer, offset + bytesCount, Buffer, HeadIndex, bytesToWrite);
                HeadIndex += count;
                Count += bytesToWrite;
            }
        }

        public void Write(byte[] buffer)
        {
            Write(buffer, 0, buffer.Length);
        }

        public void Read(byte[] buffer, int offset, int count)
        {
            // 读取长度大于数据长度
            if (count > Count)
            {
                throw new IndexOutOfRangeException("环形缓冲区读取长度大于数据长度");
            }

            int bytesToRead = count;
            int bytesCount = (TailIndex + count < Buffer.Length) ? count : (Buffer.Length - TailIndex);

            // 缓冲区读取
            Array.Copy(Buffer, TailIndex, buffer, offset, bytesCount);
            TailIndex += bytesCount;
            Count -= bytesCount;
            bytesToRead -= bytesCount;

            // 跨缓冲区边界读取
            if (bytesToRead != 0)
            {
                TailIndex = 0;
                Array.Copy(Buffer, TailIndex, buffer, offset + bytesCount, bytesToRead);
                TailIndex += count;
                Count -= bytesToRead;
            }
        }

        public void Read(byte[] buffer)
        {
            Read(buffer, 0, buffer.Length);
        }
    }
}
