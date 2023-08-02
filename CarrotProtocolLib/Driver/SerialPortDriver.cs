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
    public class SerialPortDriver
    {
        private SerialPort Driver { get; set; }

        public bool IsOpen { get; set; }

        public int BytesToRead => Driver.BytesToRead;

        public int ReceivedByteCount { get; set; }

        public int SentByteCount { get; set; }

        public delegate void ErrorReceivedHandler(Exception ex);
        public event ErrorReceivedHandler? ErrorReceived;

        public SerialPortDriver()
        {
            Driver = new SerialPort();

            Driver.ErrorReceived += Driver_ErrorReceived;
        }

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
    }

}
