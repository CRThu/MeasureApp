using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Protocols
{
    /// <summary>
    /// 数据包
    /// </summary>
    public class Packet
    {
        /// <summary>
        /// 字节数组
        /// </summary>
        public byte[] Bytes { get; set; }
        
        /// <summary>
        /// 数据包可阅读信息
        /// </summary>
        public string? Message => Encoding.ASCII.GetString(Bytes);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bytes"></param>
        public Packet(byte[] bytes)
        {
            Bytes = bytes;
        }
    }
}
