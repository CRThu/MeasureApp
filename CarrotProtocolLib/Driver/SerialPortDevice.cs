using CarrotProtocolLib.Interface;
using CarrotProtocolLib.Util;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Driver
{
    public class SerialPortDevice : IDevice
    {
        private readonly SerialPort Sp;
        private RingBuffer RxBuffer;

        public int ReceivedByteCount { get; set; }
        public int SentByteCount { get; set; }

        public bool IsOpen => Sp.IsOpen;
        public int RxByteToRead => RxBuffer.Count;

        public SerialPortDevice(string portName, int baudRate)
        {
            Sp = new(portName, baudRate)
            {
                ReadBufferSize = 65536,
                WriteBufferSize = 65536,
                ReadTimeout = 500,
                WriteTimeout = 500
            };
            Sp.DataReceived += Sp_DataReceived;
            Sp.ErrorReceived += Sp_ErrorReceived;

            RxBuffer = new(1048576);
            ReceivedByteCount = 0;
            SentByteCount = 0;
        }

        public void Open()
        {
            Sp.Open();
        }

        public void Close()
        {
            Sp.Close();
        }

        /// <summary>
        /// 发送字符串(\r\n结尾)
        /// </summary>
        /// <param name="s"></param>
        public void WriteString(string s)
        {
            Sp.WriteLine(s);
            SentByteCount += s.Length + 2;
        }

        /// <summary>
        /// 发送字节数组
        /// </summary>
        /// <param name="bytes"></param>
        public void Write(byte[] bytes)
        {
            Sp.Write(bytes, 0, bytes.Length);
            SentByteCount += bytes.Length;
        }

        /// <summary>
        /// 接收字节
        /// </summary>
        /// <param name="responseBytes"></param>
        /// <param name="bytesExpected"></param>
        /// <returns></returns>
        private int Receive(byte[] responseBytes, int bytesExpected)
        {
            int offset = 0, bytesRead;
            while (bytesExpected > 0)
            {
                bytesRead = Sp.Read(responseBytes, offset, bytesExpected);
                offset += bytesRead;
                bytesExpected -= bytesRead;
            }
            ReceivedByteCount += offset;
            return offset;
        }

        /// <summary>
        /// 接收现有字节
        /// </summary>
        /// <returns></returns>
        private byte[] Receive()
        {
            int len = Sp.BytesToRead;
            byte[] buf = new byte[len];
            int readBytes = Receive(buf, len);
            if (readBytes != len)
            {
                throw new NotImplementedException($"Read(): readBytes({readBytes}) != len({len}).");
            }
            return buf;
        }

        /// <summary>
        /// 读取字符串(\r\n结尾)
        /// </summary>
        /// <returns></returns>
        private string ReceiveString()
        {
            string rx = Sp.ReadLine();
            ReceivedByteCount += rx.Length;
            return rx;
        }

        /// <summary>
        /// 接收缓冲区读取字节
        /// </summary>
        /// <param name="responseBytes"></param>
        /// <param name="offset"></param>
        /// <param name="bytesExpected"></param>
        public void Read(byte[] responseBytes, int offset, int bytesExpected)
        {
            RxBuffer.Read(responseBytes, offset, bytesExpected);
        }

        private void Sp_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            throw new NotImplementedException($"ERROR Sp_ErrorReceived(): EventType={e.EventType}.");
        }

        private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] rx = Receive();
            RxBuffer.Write(rx);
        }
    }
}
