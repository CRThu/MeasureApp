using CarrotProtocolLib.Device;
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
    public partial class SerialPortDriver : ObservableObject, IDriver
    {
        /// <summary>
        /// 驱动层实现
        /// </summary>
        private SerialPort Driver { get; set; }

        /// <summary>
        /// 设备是否开启
        /// </summary>
        [ObservableProperty]
        private bool isOpen;

        /// <summary>
        /// 缓冲区未读取字节数
        /// </summary>
        public int BytesToRead => Driver.BytesToRead;

        /// <summary>
        /// 接收字节数
        /// </summary>
        [ObservableProperty]
        private int receivedByteCount;

        /// <summary>
        /// 发送字节数
        /// </summary>
        [ObservableProperty]
        private int sentByteCount;

        public event IDriver.ErrorReceivedHandler? ErrorReceived;

        public static int[] SupportedBaudRate { get; } = { 9600, 38400, 115200, 460800, 921600, 1000000, 2000000, 4000000, 8000000, 12000000 };
        public static int[] SupportedDataBits { get; } = { 5, 6, 7, 8 };
        public static float[] SupportedStopBits { get; } = { 0f, 1f, 1.5f, 2f };
        public static string[] SupportedParity { get; } = { "None", "Odd", "Even", "Mark", "Space" };

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
        public void SetDriver(string portName, int baudRate,
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
        public static DeviceInfo[] GetDevicesInfo()
        {
            return SerialPort.GetPortNames().Select(d => new DeviceInfo("System.IO.Ports.SerialPort", d, "串口设备")).ToArray();
        }
    }
}
