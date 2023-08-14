using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrotProtocolLib.Logger;
using CarrotProtocolLib.Util;

namespace CarrotProtocolLib.Protocol
{
    public interface IProtocolFrame
    {
        /// <summary>
        /// stream id
        /// </summary>
        public byte StreamId { get; set; }
        /// <summary>
        /// 通信协议帧字节数组
        /// </summary>
        public byte[] FrameBytes { get; }
        /// <summary>
        /// 数据负载显示
        /// </summary>
        public string PayloadDisplay { get; }

        /// <summary>
        /// 转换为Record
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public IRecord ToRecord(string from, string to);
    }
}
