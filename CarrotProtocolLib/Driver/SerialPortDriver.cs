using CarrotProtocolLib.Device;
using CarrotProtocolLib.Interface;
using CarrotProtocolLib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Driver
{
    public class SerialPortDriver : IDriver
    {
        /// <summary>
        /// 驱动层实现
        /// </summary>
        private SerialPort Driver { get; set; }

        /// <summary>
        /// 设备是否开启
        /// </summary>
        public bool IsOpen { get; set; }

        /// <summary>
        /// 缓冲区未读取字节数
        /// </summary>
        public int BytesToRead => Driver.BytesToRead;

        /// <summary>
        /// 接收字节数
        /// </summary>
        public int ReceivedByteCount { get; set; }

        /// <summary>
        /// 发送字节数
        /// </summary>
        public int SentByteCount { get; set; }

        public delegate void ErrorReceivedHandler(Exception ex);
        public event ErrorReceivedHandler? ErrorReceived;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SerialPortDriver()
        {
            Driver = new SerialPort();

            Driver.ErrorReceived += Driver_ErrorReceived;
        }

        /// <summary>
        /// 串口设置
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <param name="parity"></param>
        /// <param name="bufferSize"></param>
        /// <param name="timeout"></param>
        public void SetDevice(string portName, int baudRate,
            int dataBits, float stopBits, string parity,
            int bufferSize = 1048576, int timeout = 250)
        {
            Driver.PortName = portName;
            Driver.BaudRate = baudRate;
            Driver.DataBits = dataBits;
            Driver.StopBits = SerialPortHelper.StopBitsFloat2Enum(stopBits);
            Driver.Parity = SerialPortHelper.ParityString2Enum(parity);
            Driver.ReadBufferSize = bufferSize;
            Driver.WriteBufferSize = bufferSize;
            Driver.ReadTimeout = timeout;
            Driver.WriteTimeout = timeout;
        }

        /// <summary>
        /// 串口内部错误捕获
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Driver_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            ErrorReceived?.Invoke(new DriverErrorException(this, $"ErrorReceived, EventType={e.EventType}."));
            IsOpen = Driver.IsOpen;
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        public void Open()
        {
            try
            {
                Driver.Open();
                IsOpen = Driver.IsOpen;
            }
            catch (Exception ex)
            {
                IsOpen = Driver.IsOpen;
                throw new DriverErrorException(this, ex.Message, ex);
            }
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void Close()
        {
            try
            {
                Driver.Close();
                IsOpen = Driver.IsOpen;
            }
            catch (Exception ex)
            {
                IsOpen = Driver.IsOpen;
                throw new DriverErrorException(this, ex.Message, ex);
            }
        }

        /// <summary>
        /// 写入字节数组
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void Write(byte[] buffer, int offset, int count)
        {
            try
            {
                Driver.Write(buffer, offset, count);
                SentByteCount += count;
            }
            catch (Exception ex)
            {
                IsOpen = Driver.IsOpen;
                throw new DriverErrorException(this, ex.Message, ex);
            }
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
            try
            {
                int bytesRead;
                while (bytesExpected > 0)
                {
                    bytesRead = Driver.Read(buffer, offset, bytesExpected);
                    offset += bytesRead;
                    bytesExpected -= bytesRead;
                }
                ReceivedByteCount += offset;
                return offset;
            }
            catch (Exception ex)
            {
                IsOpen = Driver.IsOpen;
                throw new DriverErrorException(this, ex.Message, ex);
            }
        }

        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        public DeviceInfo[] GetDevicesInfo()
        {
            return SerialPort.GetPortNames().Select(d => new DeviceInfo("System.IO.Ports.SerialPort", d, "串口设备")).ToArray();
        }
    }
}
