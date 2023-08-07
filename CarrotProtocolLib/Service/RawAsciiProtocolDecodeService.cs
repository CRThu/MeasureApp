using CarrotProtocolLib.Device;
using CarrotProtocolLib.Impl;
using CarrotProtocolLib.Logger;
using CarrotProtocolLib.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Service
{
    /// <summary>
    /// 原始字符串流解析
    /// </summary>
    public class RawAsciiProtocolDecodeService : ProtocolDecodeBaseService
    {
        /// <summary>
        /// 操作设备接口
        /// </summary>
        private IDevice? Device { get; set; }

        /// <summary>
        /// 数据帧存储接口
        /// </summary>
        private ILogger? Logger { get; set; }

        /// <summary>
        /// 解码缓冲区(内部使用)
        /// </summary>
        private byte[] DecodeBuffer { get; set; }

        /// <summary>
        /// 解码缓冲区游标(内部使用)
        /// </summary>
        private int DecodeBufferCursor { get; set; }
        /// <summary>
        /// 解码缓冲区游标,标记有效开始位置(内部使用)
        /// </summary>
        private int DecodeBufferStartCursor { get; set; }

        /// <summary>
        /// 数据帧存储缓冲区(内部使用)
        /// </summary>
        private byte[] FrameBuffer { get; set; }

        /// <summary>
        /// 数据帧存储游标(内部使用)
        /// </summary>
        private int FrameBufferCursor { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public RawAsciiProtocolDecodeService() : base()
        {
            DecodeBuffer = new byte[64 * 1024];
            DecodeBufferCursor = 0;
            FrameBuffer = Array.Empty<byte>();
            FrameBufferCursor = 0;
            DecodeBufferStartCursor = 0;
        }

        /// <summary>
        /// 数据接收服务程序
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override int ServiceLoop()
        {
            // 大于requiredBytesToRead时进入搜索是否存在字符串结束字符
            int requiredBytesToRead = 1;
            // 已搜索过结束字符字符串位置
            int findFrameEndByteCursor = 0;
            // 缓冲区有效数据开始位置
            DecodeBufferStartCursor = 0;
            while (true)
            {
                // 外部退出请求检测
                if (IsCancellationRequested)
                    return 1;

                // 获取缓冲区数据
                int len = Device!.RxByteToRead;
                // 读取不超过缓冲区空间的字节数
                if (len > DecodeBuffer.Length - DecodeBufferCursor)
                    len = DecodeBuffer.Length - DecodeBufferCursor;

                if (len >= requiredBytesToRead)
                {
                    // 数据存入DecodeBuffer
                    Device.Read(DecodeBuffer, DecodeBufferCursor, len);
                    DecodeBufferCursor += len;

                    // 数据解析
                    while (findFrameEndByteCursor < DecodeBufferCursor)
                    {
                        int index = Array.IndexOf(DecodeBuffer, (byte)'\n', findFrameEndByteCursor);

                        // 查找到结束字符
                        if (index >= 0)
                        {
                            Array.Copy(DecodeBuffer, DecodeBufferStartCursor, FrameBuffer, 0, index + 1 - DecodeBufferStartCursor);
                            Logger!.AddRx(new RawAsciiProtocolRecord(FrameBuffer, 0, index + 1 - DecodeBufferStartCursor));
                            DecodeBufferStartCursor = index + 1;
                            findFrameEndByteCursor = index + 1;
                        }
                        // 未搜索到结束字符
                        else if (DecodeBufferStartCursor == 0)
                        {
                            findFrameEndByteCursor = DecodeBufferCursor;
                        }
                        // 已解码多个包，最后一个不完整包无结束字符
                        else
                        {
                            Array.Copy(DecodeBuffer, DecodeBufferStartCursor, DecodeBuffer, 0, DecodeBufferCursor - DecodeBufferStartCursor);
                            Array.Fill(DecodeBuffer, (byte)0x00, DecodeBufferCursor - DecodeBufferStartCursor, DecodeBuffer.Length - (DecodeBufferCursor - DecodeBufferStartCursor));

                            DecodeBufferCursor -= DecodeBufferStartCursor;
                            DecodeBufferStartCursor = 0;
                            findFrameEndByteCursor = 0;
                        }
                    }
                }
                else
                {
                    Thread.Sleep(5);
                }
                return 0;
            }

        }
    }
}
