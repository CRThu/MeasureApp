using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Protocols
{
    public class Packet
    {
        public byte[] Bytes { get; set; }
        public string? Message => Encoding.ASCII.GetString(Bytes);

        public Packet(byte[] bytes)
        {
            Bytes = bytes;
        }
    }
}
