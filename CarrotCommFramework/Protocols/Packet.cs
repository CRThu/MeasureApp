using CarrotCommFramework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Protocols
{
    /// <summary>
    /// 数据包
    /// </summary>
    public class Packet
    {
        public static Packet Empty => new([]);

        /// <summary>
        /// 字节数组
        /// </summary>
        public virtual byte[]? Bytes { get; set; }

        /// <summary>
        /// 数据包可阅读信息
        /// </summary>
        public virtual string? Message => null;
        public virtual byte? ProtocolId => null;
        public virtual byte? StreamId => null;

        public Packet()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Packet(byte[] bytes)
        {
            Bytes = bytes;
        }

    }

    public interface IMessagePacket
    {
        public byte[] Encode(string msg);

        public string Decode(byte[] bytes);
    }

    public interface IRegisterPacket
    {
        public byte[] Encode(int oper, int regfile, int addr, int data);

        public (int control, int regfile, int addr, int data) Decode(byte[] bytes);
}
}
