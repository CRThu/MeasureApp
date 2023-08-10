using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrotProtocolLib.Util;

namespace CarrotProtocolLib.Protocol
{
    public interface IProtocolFrame
    {
        /// <summary>
        /// 通信协议帧字节数组
        /// </summary>
        public byte[] FrameBytes { get; }
        /// <summary>
        /// 数据负载显示
        /// </summary>
        public string PayloadDisplay { get; }
    }
}
