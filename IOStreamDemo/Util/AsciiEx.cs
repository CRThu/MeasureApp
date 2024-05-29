using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Util
{
    public static class AsciiEx
    {
        /// <summary>
        /// 检测是否为可打印字符
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsPrintableAscii(this char c)
        {
            return c >= (char)0x20 && c <= (char)0x7E;
        }

        /// <summary>
        /// 检测是否为可打印字符
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsPrintableAscii(this byte c)
        {
            return c >= (byte)0x20 && c <= (byte)0x7E;
        }

        /// <summary>
        /// 是否为16进制字符
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsHexAscii(this char c)
        {
            return HexCharToNum(c) != 255;
        }

        /// <summary>
        /// 16进制字符char转数字
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static byte HexCharToNum(this char c)
        {
            return c switch
            {
                '0' => 0,
                '1' => 1,
                '2' => 2,
                '3' => 3,
                '4' => 4,
                '5' => 5,
                '6' => 6,
                '7' => 7,
                '8' => 8,
                '9' => 9,
                'A' => 10,
                'B' => 11,
                'C' => 12,
                'D' => 13,
                'E' => 14,
                'F' => 15,
                'a' => 10,
                'b' => 11,
                'c' => 12,
                'd' => 13,
                'e' => 14,
                'f' => 15,
                _ => 255
            };
        }
    }
}
