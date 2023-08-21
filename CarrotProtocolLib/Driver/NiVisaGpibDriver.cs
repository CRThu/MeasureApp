using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CarrotProtocolLib.Device;
using CommunityToolkit.Mvvm.ComponentModel;
using NationalInstruments.VisaNS;

namespace CarrotProtocolLib.Driver
{
    public partial class NiVisaDriver : ObservableObject, IDriver
    {
        /// <summary>
        /// 驱动层实现
        /// </summary>
        private MessageBasedSession Session { get; set; }

        private string Addr { get; set; }

        /// <summary>
        /// 设备是否开启
        /// </summary>
        [ObservableProperty]
        private bool isOpen;

        /// <summary>
        /// 缓冲区未读取字节数
        /// </summary>
        public int BytesToRead => 0;

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

        public NiVisaDriver()
        {
        }


        public void SetDriver(string addr)
        {
            Addr = addr;
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        public void Open()
        {
            try
            {
                Session = (MessageBasedSession)ResourceManager.GetLocalManager().Open(Addr);
                IsOpen = true;
            }
            catch (Exception ex)
            {
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
                Session.Dispose();
                IsOpen = false;
            }
            catch (Exception ex)
            {
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
                byte[] bytes = new byte[count];
                Array.Copy(buffer, offset, bytes, 0, count);
                Session.Write(bytes);
                SentByteCount += count;
            }
            catch (Exception ex)
            {
                throw new DriverErrorException(this, ex.Message, ex);
            }
        }

        /// <summary>
        /// 读取字节数组
        /// </summary>
        public int Read(byte[] buffer, int offset, int bytesExpected)
        {
            //Session.ReadStatusByte();

            try
            {
                byte[] x = Session.ReadByteArray();
                int bytesRead = x.Length <= bytesExpected ? x.Length : bytesExpected;
                Array.Copy(x, 0, buffer, offset, bytesRead);
                ReceivedByteCount += bytesRead;
                return bytesRead;
            }
            catch (Exception ex)
            {
                throw new DriverErrorException(this, ex.Message, ex);
            }
        }

        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        public static DeviceInfo[] GetDevicesInfo()
        {
            // Reference: documentation.help/VISA.NET/VISA_Attibutes.htm
            // INSTR: Instrument Control
            // INTFC: GPIB Bus Interface
            try
            {
                string expression = "GPIB?*INSTR";
                string[] res = ResourceManager.GetLocalManager().FindResources(expression);
                return res.Select(d => new DeviceInfo("DEVICE", d, "NI-VISA DEVICE")).ToArray();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                Debug.WriteLine(ex.ToString());
                return Array.Empty<DeviceInfo>();
            }
        }
    }
}
