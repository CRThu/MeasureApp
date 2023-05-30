using CarrotProtocolLib.Interface;
using CarrotProtocolLib.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.Security.Cryptography;

namespace CarrotProtocolLib.Device
{
    public partial class SerialPortDevice : ObservableObject, IDevice
    {
        private SerialPort Sp { get; set; }
        private RingBuffer RxBuffer { get; set; }

        [ObservableProperty]
        public int receivedByteCount;
        [ObservableProperty]
        public int sentByteCount;

        [ObservableProperty]
        public bool isOpen;

        public int RxByteToRead => RxBuffer.Count;
        private CancellationTokenSource DataReceiveTaskCts { get; set; }
        private Task<int> DataReceiveTask { get; set; }


        public delegate void ReceiveErrorHandler(Exception ex);
        public event ReceiveErrorHandler ReceiveError;

        //public delegate void OnInternalPropertyChangedHandler(string name, dynamic value);
        public event IDevice.OnInternalPropertyChangedHandler InternalPropertyChanged;

        public static int[] SupportedBaudRate { get; } = { 9600, 38400, 115200, 460800, 921600, 1000000, 2000000, 4000000, 8000000, 12000000 };
        public static int[] SupportedDataBits { get; } = { 5, 6, 7, 8 };
        public static float[] SupportedStopBits { get; } = { 0f, 1f, 1.5f, 2f };
        public static string[] SupportedParity { get; } = { "None", "Odd", "Even", "Mark", "Space" };


        partial void OnIsOpenChanged(bool value)
        {
            InternalPropertyChanged?.Invoke(nameof(IsOpen), value);
        }

        partial void OnReceivedByteCountChanged(int value)
        {
            InternalPropertyChanged?.Invoke(nameof(ReceivedByteCount), value);
        }

        partial void OnSentByteCountChanged(int value)
        {
            InternalPropertyChanged?.Invoke(nameof(SentByteCount), value);
        }

        public SerialPortDevice()
        {
            IsOpen = false;
        }

        public void SetDevice(string portName, int baudRate, int dataBits, float stopBits, string parity)
        {
            Sp = new()
            {
                PortName = portName,
                BaudRate = baudRate,
                DataBits = dataBits,
                StopBits = StopBitsFloat2Enum(stopBits),
                Parity = ParityString2Enum(parity),
                ReadBufferSize = 1048576,
                WriteBufferSize = 1048576,
                ReadTimeout = 2000,
                WriteTimeout = 2000
            };
            //Sp.DataReceived += Sp_DataReceived;
            Sp.ErrorReceived += Sp_ErrorReceived;

            RxBuffer = new(1048576 * 16);
            ReceivedByteCount = 0;
            SentByteCount = 0;

            IsOpen = false;
        }

        public static DeviceInfo[] GetDevicesInfo()
        {
            return SerialPort.GetPortNames().Select(d => new DeviceInfo(InterfaceType.SerialPort, d, "串口设备")).ToArray();
        }

        public void Open()
        {
            Sp.Open();
            DataReceiveTaskCts = new();
            DataReceiveTask = Task.Run(() => DataReceiveLoop(), DataReceiveTaskCts.Token);
            IsOpen = true;
        }

        public void Close()
        {
            Sp.Close();
            DataReceiveTaskCts.Cancel();
            DataReceiveTask.Wait();
            IsOpen = false;

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
            ReceiveError?.Invoke(new NotImplementedException($"ERROR Sp_ErrorReceived(): EventType={e.EventType}."));
            IsOpen = Sp.IsOpen;
        }

        private int DataReceiveLoop()
        {
            try
            {
                while (IsOpen)
                {
                    if (DataReceiveTaskCts.Token.IsCancellationRequested)
                        return 0;

                    int len = Sp.BytesToRead;
                    if (len > 0)
                    {
                        byte[] buf = new byte[len];
                        int readBytes = Receive(buf, len);
                        if (readBytes != len)
                        {
                            throw new NotImplementedException($"Read(): readBytes({readBytes}) != len({len}).");
                        }
                        // Debug.WriteLine($"BytesToRead = {len}, Read = {readBytes}, ReceivedByteCount = {ReceivedByteCount}");
                        //int len2 = Sp.BytesToRead;
                        //Debug.WriteLine($"BytesToRead2 = {len2}");
                        if (buf.Length > 0)
                        {
                            RxBuffer.Write(buf);
                            Debug.WriteLine($"buflen = {buf.Length}");
                        }
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                ReceiveError?.Invoke(ex);
                return -1;
            }
        }

        public static StopBits StopBitsFloat2Enum(float stopBits)
        {
            return stopBits switch
            {
                0f => StopBits.None,
                1f => StopBits.One,
                2f => StopBits.Two,
                1.5f => StopBits.OnePointFive,
                _ => throw new NotImplementedException($"不支持的停止位: {stopBits:0.0}"),
            };
        }

        public static Parity ParityString2Enum(string parity)
        {
            return parity switch
            {
                "None" => Parity.None,
                "Odd" => Parity.Odd,
                "Even" => Parity.Even,
                "Mark" => Parity.Mark,
                "Space" => Parity.Space,
                _ => throw new NotImplementedException($"不支持的校验位: {parity}"),
            };
        }
    }
}
