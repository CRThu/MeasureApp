using CarrotProtocolLib.Interface;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using FTD2XX_NET;
using System.IO.Ports;
using CarrotProtocolLib.Util;

namespace CarrotProtocolLib.Device
{
    public partial class FtdiD2xxDevice : ObservableObject, IDevice
    {
        public IDriver Driver { get; set; }
        public RingBuffer RxBuffer { get; set; }

        /// <summary>
        /// 缓冲区待接收的数据字节数
        /// </summary>
        public int RxByteToRead => RxBuffer.Count;

        /// <summary>
        /// 接收数据字节数
        /// </summary>
        public int ReceivedByteCount => Driver.ReceivedByteCount;

        /// <summary>
        /// 发送数据字节数
        /// </summary>
        public int SentByteCount => Driver.SentByteCount;

        /// <summary>
        /// 设备是否打开
        /// </summary>
        public bool IsOpen => Driver.IsOpen;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="bufferSize"></param>
        public FtdiD2xxDevice(IDriver driver, int bufferSize = 1048576 * 16)
        {
            Driver = driver;
            //RxBuffer = new(1048576 * 16);
            RxBuffer = new(bufferSize);
        }

        /// <summary>
        /// 打开设备
        /// </summary>
        public void Open()
        {
            Driver.Open();
            RxBuffer.Clear();
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        public void Close()
        {
            Driver.Close();
        }

        /// <summary>
        /// 写入字节数组
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void Write(byte[] buffer, int offset, int count)
        {
            Driver.Write(buffer, offset, count);
        }

        /// <summary>
        /// 读取字节数组
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="bytesExpected"></param>
        /// <returns>返回实际读取字节数</returns>
        public int Read(byte[] buffer, int offset, int bytesExpected)
        {
            if (bytesExpected > RxByteToRead)
                bytesExpected = RxByteToRead;
            RxBuffer.Read(buffer, offset, bytesExpected);
            return bytesExpected;
        }

    }
}
