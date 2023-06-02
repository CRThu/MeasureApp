using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Util
{
    public static class AsciiEx
    {
        public static bool IsPrintableAscii(this char c)
        {
            return c >= (char)0x20 && c <= (char)0x7E;
        }

        public static bool IsPrintableAscii(this byte c)
        {
            return c >= (byte)0x20 && c <= (byte)0x7E;
        }
    }
}
