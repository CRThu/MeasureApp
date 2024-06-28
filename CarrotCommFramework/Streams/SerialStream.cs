using HighPrecisionTimer;
using CarrotCommFramework.Util;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Streams
{
    public class SerialStream : StreamBase
    {
        public SerialStream()
        {
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
        public override void Config(IDictionary<string, string> @params = default!)
        {
            if (@params.Count == 0)
                return;

            Driver = new SerialPort();
            //Driver.ReadTimeout = 10;
            //Driver.WriteTimeout = 10;

            Driver.PortName = @params.TryGetValue("port", out string? value1) ? value1 : string.Empty;
            Driver.BaudRate = @params.TryGetValue("baudrate", out string? value2) ? Convert.ToInt32(value2) : 115200;
            Driver.DataBits = @params.TryGetValue("databits", out string? value3) ? Convert.ToInt32(value3) : 8;
            Driver.Parity = @params.TryGetValue("parity", out string? value4) ? SerialPortHelper.ParityString2Enum(value4) : Parity.None;
            Driver.StopBits = @params.TryGetValue("stopbits", out string? value5) ? SerialPortHelper.StopBitsFloat2Enum(Convert.ToDouble(value5)) : StopBits.One;
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
