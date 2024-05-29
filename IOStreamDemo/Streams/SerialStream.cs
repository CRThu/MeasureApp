using HighPrecisionTimer;
using IOStreamDemo.Util;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Streams
{
    public class SerialStream : StreamBase
    {
        public SerialStream(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 流指示有数据
        /// </summary>
        public override bool ReadAvailable => Driver.BytesToRead > 0;

        /// <summary>
        /// 驱动层实现
        /// </summary>
        private SerialPort Driver { get; set; } = new();

        /// <summary>
        /// 配置解析和初始化
        /// </summary>
        /// <param name="params"></param>
        public override void Config(string[] @params = default!)
        {
            if (@params.Length == 0)
                return;

            Driver = new SerialPort();
            //Driver.ReadTimeout = 10;
            //Driver.WriteTimeout = 10;

            if (@params.Length > 0)
                Driver.PortName = @params[0];
            if (@params.Length > 1)
                Driver.BaudRate = Convert.ToInt32(@params[1]);
            if (@params.Length > 2)
                Driver.DataBits = Convert.ToInt32(@params[2]);
            if (@params.Length > 3)
                Driver.Parity = SerialPortHelper.ParityString2Enum(@params[3]);
            if (@params.Length > 4)
                Driver.StopBits = SerialPortHelper.StopBitsFloat2Enum(Convert.ToDouble(@params[4]));
        }

        /// <summary>
        /// 关闭流
        /// </summary>
        public override void Close()
        {
            Driver.Close();
        }

        /// <summary>
        /// 打开流
        /// </summary>
        public override void Open()
        {
            Driver.Open();
        }

        /// <summary>
        /// 流写入
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            Driver.BaseStream.Write(buffer, offset, count);
        }

        /// <summary>
        /// 流读取
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            // TODO 同步流读取存在阻塞，待优化
            return Driver.BaseStream.Read(buffer, offset, count);
        }
    }
}
