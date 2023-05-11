using CarrotProtocolLib.Interface;
using CarrotProtocolLib.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Driver
{
    public class SerialPortDevice : IDevice
    {
        private SerialPort Sp { get; set; }
        private RingBuffer RxBuffer { get; set; }

        public int ReceivedByteCount { get; set; }
        public int SentByteCount { get; set; }

        public bool IsOpen => Sp is not null && Sp.IsOpen;
        public int RxByteToRead => RxBuffer.Count;
        private CancellationTokenSource DataReceiveTaskCts { get; set; }
        private Task<int> DataReceiveTask { get; set; }


        public delegate void ReceiveErrorHandler(Exception ex);
        public event ReceiveErrorHandler ReceiveError;

        public SerialPortDevice()
        {
        }

        public void SetDevice(string portName, int baudRate)
        {
            Sp = new()
            {
                PortName = portName,
                BaudRate = baudRate,
                ReadBufferSize = 1048576 * 16,
                WriteBufferSize = 1048576 * 16,
                ReadTimeout = 2000,
                WriteTimeout = 2000
            };
            //Sp.DataReceived += Sp_DataReceived;
            Sp.ErrorReceived += Sp_ErrorReceived;

            RxBuffer = new(1048576 * 16);
            ReceivedByteCount = 0;
            SentByteCount = 0;
        }

        public void Open()
        {
            Sp.Open();
            DataReceiveTaskCts = new();
            DataReceiveTask = Task.Run(() => DataReceiveLoop(), DataReceiveTaskCts.Token);
        }

        public void Close()
        {
            Sp.Close();
            DataReceiveTaskCts.Cancel();
            DataReceiveTask.Wait();
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
            Debug.WriteLine($"BytesToRead = {len}, Read = {readBytes}, ReceivedByteCount = {ReceivedByteCount}");
            int len2 = Sp.BytesToRead;
            Debug.WriteLine($"BytesToRead2 = {len2}");
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

        private int DataReceiveLoop()
        {
            try
            {
                while (IsOpen)
                {
                    if (DataReceiveTaskCts.Token.IsCancellationRequested)
                        return 0;

                    byte[] rx = Receive();
                    RxBuffer.Write(rx);
                }
                return 1;
            }
            catch (Exception ex)
            {
                ReceiveError?.Invoke(ex);
                return -1;
            }
        }
    }
}
