using CarrotProtocolLib.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CarrotProtocolLib.Driver.SerialPortDriver;

namespace CarrotProtocolLib.Interface
{
    public interface IDriver
    {
        /// <summary>
        /// 设备是否开启
        /// </summary>
        public bool IsOpen { get; }

        /// <summary>
        /// 缓冲区未读取字节数
        /// </summary>
        public int BytesToRead { get; }

        /// <summary>
        /// 接收字节数
        /// </summary>
        public int ReceivedByteCount { get; }

        /// <summary>
        /// 发送字节数
        /// </summary>
        public int SentByteCount { get; }

        public delegate void ErrorReceivedHandler(Exception ex);
        public event ErrorReceivedHandler? ErrorReceived;


        /// <summary>
        /// 打开串口
        /// </summary>
        public void Open();
        /// <summary>
        /// 关闭串口
        /// </summary>
        public void Close();
        /// <summary>
        /// 写入字节数组
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void Write(byte[] buffer, int offset, int count);
        /// <summary>
        /// 读取字节数组
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="bytesExpected"></param>
        /// <returns>返回实际读取字节数</returns>
        public int Read(byte[] buffer, int offset, int bytesExpected);
        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        public DeviceInfo[] GetDevicesInfo();
    }
}
