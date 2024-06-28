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
using NationalInstruments.VisaNS;
using CarrotCommFramework.Sessions;
using System.Diagnostics;

namespace CarrotCommFramework.Streams
{
    public class NiVisaStream : StreamBase
    {

        /// <summary>
        /// 流指示有数据
        /// </summary>
        public override bool ReadAvailable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 驱动层实现
        /// </summary>
        protected MessageBasedSession Session { get; set; }

        protected string Addr { get; set; }

        public NiVisaStream()
        {
        }

        /// <summary>
        /// 配置解析和初始化
        /// </summary>
        /// <param name="params"></param>
        public override void Config(IDictionary<string, string> @params = default!)
        {
            Addr = @params.TryGetValue("address", out string? value) ? value : string.Empty;
        }

        /// <summary>
        /// 关闭流
        /// </summary>
        public override void Close()
        {
            Session.Dispose();
        }

        /// <summary>
        /// 打开流
        /// </summary>
        public override void Open()
        {
            var res = (MessageBasedSession)ResourceManager.GetLocalManager().Open(Addr);
            if (res is MessageBasedSession)
                Session = res;
            else
                throw new Exception();

            Session.Timeout = 30000;
        }

        /// <summary>
        /// 流写入
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            byte[] bytes = new byte[count];
            Array.Copy(buffer, offset, bytes, 0, count);
            Session.Write(buffer);
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
            try
            {
                byte[] x = Session.ReadByteArray(count);
                Array.Copy(x, 0, buffer, offset, x.Length);
                return x.Length;
            }
            catch
            {
                return 0;
            }
        }
    }
}
