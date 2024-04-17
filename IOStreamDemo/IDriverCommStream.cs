using CarrotProtocolLib.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo
{
    public enum StreamStatus
    {
        Initial,
        Close,
        Open
    }

    /// <summary>
    /// 驱动层抽象通信流
    /// </summary>
    public interface IDriverCommStream
    {
        public DeviceInfo DeviceInfo { get; set; }

        public StreamStatus Status { get; set; }

        public ulong RxCount { get; set; }
        public ulong TxCount { get; set; }

        public delegate void StreamErrorReceivedHandler(Exception ex);
        public event StreamErrorReceivedHandler? StreamErrorReceived;

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="addr"></param>
        public abstract void Open(string addr);
        /// <summary>
        /// 关闭串口
        /// </summary>
        public abstract void Close();
        /// <summary>
        /// 写入字节数组
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public abstract void Write(Span<byte> buffer, int offset, int count);
        /// <summary>
        /// 读取字节数组
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="bytesExpected"></param>
        /// <returns>返回实际读取字节数</returns>
        public abstract int Read(Span<byte> buffer, int offset, int bytesExpected);

    }
}
