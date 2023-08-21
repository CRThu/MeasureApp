using CarrotProtocolLib.Driver;
using CarrotProtocolLib.Logger;
using CarrotProtocolLib.Protocol;
using CarrotProtocolLib.Service;
using CarrotProtocolLib.Util;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Device
{
    public interface IDevice
    {
        /// <summary>
        /// 设备驱动层
        /// </summary>
        public IDriver Driver { get; set; }

        /// <summary>
        /// 记录器
        /// </summary>
        public ProtocolLogger Logger { get; set; }

        /// <summary>
        /// 缓冲区
        /// </summary>
        public RingBuffer RxBuffer { get; set; }

        /// <summary>
        /// 缓冲区待接收的数据字节数
        /// </summary>
        public int RxByteToRead { get; }

        /// <summary>
        /// 设备打开
        /// </summary>
        public void Open();

        /// <summary>
        /// 设备关闭
        /// </summary>
        public void Close();

        /// <summary>
        /// 设备写入字节流
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void Write(byte[] buffer, int offset, int count);

        /// <summary>
        /// 设备写入数据协议帧
        /// </summary>
        /// <typeparam name="T">具有IProtocolFrame接口的数据协议帧</typeparam>
        /// <param name="frame">数据协议帧实例</param>
        public void Write<T>(T frame) where T : IProtocolFrame;

        /// <summary>
        /// 设备读取字节流存储到数组位置
        /// </summary>
        /// <param name="buffer">接收数据存储数组</param>
        /// <param name="offset">偏移量</param>
        /// <param name="bytesExpected">请求读取数量</param>
        public int Read(byte[] buffer, int offset, int bytesExpected);
    }

}
