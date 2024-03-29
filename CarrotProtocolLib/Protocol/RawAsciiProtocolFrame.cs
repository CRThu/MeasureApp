﻿using CarrotProtocolLib.Logger;
using CarrotProtocolLib.Util;

namespace CarrotProtocolLib.Protocol
{
    public partial class RawAsciiProtocolFrame : IProtocolFrame
    {
        /// <summary>
        /// stream id, default: 0x00
        /// </summary>
        public byte StreamId { get; set; }

        /// <summary>
        /// payload 字节数组存储
        /// </summary>
        //[ObservableProperty]
        //[NotifyPropertyChangedFor(nameof(PayloadDisplay))]
        //[NotifyPropertyChangedFor(nameof(FrameBytes))]
        private readonly byte[] PayloadBytes;

        /// <summary>
        /// payload 显示转义字符串
        /// </summary>
        public string PayloadDisplay
        {
            get
            {
                return PayloadBytes.BytesToEscapeString();
            }
        }

        /// <summary>
        /// payload 帧数据(以\n结尾)
        /// </summary>
        public byte[] FrameBytes
        {
            get
            {
                if (PayloadBytes[^1] == '\n')
                    return PayloadBytes;
                else
                    return PayloadBytes.Concat("\r\n"u8.ToArray()).ToArray();
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="s">字符串</param>
        public RawAsciiProtocolFrame(string s)
        {
            PayloadBytes = s.AsciiToBytes();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="offset">起始偏移量</param>
        /// <param name="length">长度</param>
        public RawAsciiProtocolFrame(byte[] bytes, int offset, int length)
        {
            byte[] bytesNew = new byte[length];
            Array.Copy(bytes, offset, bytesNew, 0, length);
            PayloadBytes = bytesNew;
        }

        public IRecord ToRecord(string from, string to)
        {
            return new ProtocolRecord()
            {
                Time = DateTime.Now,
                From = from,
                To = to,
                Stream = StreamId,
                ProtocolName = nameof(RawAsciiProtocolFrame),
                Type = TransferType.Command,
                Frame = this
            };
        }

        /// <summary>
        /// 解析为数据
        /// </summary>
        /// <returns></returns>
        public double[] DecodeData()
        {
            return Array.Empty<double>();
        }
    }
}
