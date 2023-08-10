using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Util
{

    // https://blog.csdn.net/qq_25482087/article/details/87925962

    public static class BytesEx
    {
        /// <summary>
        /// 检查16进制字符串格式是否符合转换要求<br/>
        /// 正确格式: 0123 ABCD abcd<br/>
        /// 分隔符支持空格<br/>
        /// 错误格式: 长度为单数/出现非0-9,a-f,A-F字符<br/>
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool CheckHexString(this string s)
        {
            s = s.Replace(" ", "");
            return s.All(c => c.IsHexAscii()) && (s.Length % 2 == 0);
        }


        /// <summary>
        /// Convert a string of hex digits (ex: E4 CA B2) to a byte array.
        /// <code>
        /// BytesEx.HexStringToBytes("12 34 56 78 90") = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90 }
        /// </code>
        /// </summary>
        /// <param name="s"> The string containing the hex digits (with or without spaces). </param>
        /// <returns> Returns an array of bytes. </returns>
        public static byte[] HexStringToBytes(this string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
            {
                buffer[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
                i += 2;
            }

            return buffer;
        }

        /// <summary>
        /// Converts an array of bytes into a formatted string of hex digits (ex: E4 CA B2)
        /// <code>
        /// BytesEx.BytesToHexString(new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90}) = "12 34 56 78 90"
        /// </code>
        /// </summary>
        /// <param name="data"> The array of bytes to be translated into a string of hex digits. </param>
        /// <returns> Returns a well formatted string of hex digits with spacing. </returns>
        public static string BytesToHexString(this byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", " ");
        }

        /// <summary>
        /// 将转换为ASCII字符串
        /// <code>
        /// BytesEx.BytesToAscii(new byte[] { 0x30, 0x31, 0x32, 0x33, 0x34 }) = "01234"
        /// </code>
        /// </summary>
        /// <param name="arrInput">byte型数组</param>
        /// <returns>目标字符串</returns>
        public static string BytesToAscii(this byte[] arrInput)
        {
            return Encoding.ASCII.GetString(arrInput);
        }

        /// <summary>
        /// 将ASCII字符串转换为byte[]
        /// <code>
        /// BytesEx.AsciiToBytes("01234") = new byte[] { 0x30, 0x31, 0x32, 0x33, 0x34}
        /// </code>
        /// </summary>
        /// <param name="asciis"></param>
        /// <returns></returns>
        public static byte[] AsciiToBytes(this string asciis)
        {
            return Encoding.ASCII.GetBytes(asciis);
        }

        /// <summary>
        /// 将一条16进制字符串转换为ASCII
        /// </summary>
        /// <param name="hexstring">一条十六进制字符串</param>
        /// <returns>返回一条ASCII码</returns>
        public static string HexStringToASCII(this string hexstring)
        {
            byte[] bt = HexStringToBinary(hexstring);
            string lin = "";
            for (int i = 0; i < bt.Length; i++)
            {
                lin = lin + bt[i] + " ";
            }


            string[] ss = lin.Trim().Split(new char[] { ' ' });
            char[] c = new char[ss.Length];
            int a;
            for (int i = 0; i < c.Length; i++)
            {
                a = Convert.ToInt32(ss[i]);
                c[i] = Convert.ToChar(a);
            }

            string b = new string(c);
            return b;
        }

        /// <summary>
        /// 16进制字符串转换为二进制数组
        /// </summary>
        /// <param name="hexstring">用空格切割字符串</param>
        /// <returns>返回一个二进制字符串</returns>
        public static byte[] HexStringToBinary(this string hexstring)
        {

            string[] tmpary = hexstring.Trim().Split(' ');
            byte[] buff = new byte[tmpary.Length];
            for (int i = 0; i < buff.Length; i++)
            {
                buff[i] = Convert.ToByte(tmpary[i], 16);
            }
            return buff;
        }


        /// <summary>
        /// int转byte[]
        /// 该方法将一个int类型的数据转换为byte[]形式，因为int为32bit，而byte为8bit所以在进行类型转换时，知会获取低8位，
        /// 丢弃高24位。通过位移的方式，将32bit的数据转换成4个8bit的数据。注意 &0xff，在这当中，&0xff简单理解为一把剪刀，
        /// 将想要获取的8位数据截取出来。
        /// </summary>
        /// <param name="i">一个int数字</param>
        /// <returns>byte[]</returns>
        public static byte[] Int2Bytes(this int i)
        {
            byte[] result = new byte[4];
            result[0] = (byte)(i >> 24 & 0xFF);
            result[1] = (byte)(i >> 16 & 0xFF);
            result[2] = (byte)(i >> 8 & 0xFF);
            result[3] = (byte)(i & 0xFF);
            return result;
        }

        /// <summary>
        /// byte[]转int
        /// 利用int2ByteArray方法，将一个int转为byte[]，但在解析时，需要将数据还原。同样使用移位的方式，将适当的位数进行还原，
        /// 0xFF为16进制的数据，所以在其后每加上一位，就相当于二进制加上4位。同时，使用|=号拼接数据，将其还原成最终的int数据
        /// </summary>
        /// <param name="bytes">byte类型数组</param>
        /// <returns>int数字</returns>
        public static int Bytes2Int(this byte[] bytes)
        {
            int num = bytes[3] & 0xFF;
            num |= bytes[2] << 8 & 0xFF00;
            num |= bytes[1] << 16 & 0xFF0000;
            num |= bytes[0] << 24 & 0xFF0000;
            return num;
        }

        public static string Int2String(this int str)
        {
            string S = Convert.ToString(str);
            return S;
        }

        public static int String2Int(this string str)
        {
            int a;
            int.TryParse(str, out a);
            int a1 = Convert.ToInt32(str);
            return a1;
        }

        /// <summary>
        /// 将int转为低字节在后，高字节在前的byte数组
        /// b[0] = 11111111(0xff) & 01100001
        /// b[1] = 11111111(0xff) & 00000000
        /// b[2] = 11111111(0xff) & 00000000
        /// b[3] = 11111111(0xff) & 00000000
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] IntToBytes2(this int value)
        {
            byte[] src = new byte[4];
            src[0] = (byte)(value >> 24 & 0xFF);
            src[1] = (byte)(value >> 16 & 0xFF);
            src[2] = (byte)(value >> 8 & 0xFF);
            src[3] = (byte)(value & 0xFF);
            return src;
        }

        /// <summary>
        /// 将高字节在前转为int，低字节在后的byte数组(与IntToByteArray2想对应)
        /// </summary>
        /// <param name="bArr"></param>
        /// <returns></returns>
        public static int BytesToInt2(this byte[] bArr)
        {
            if (bArr.Length != 4)
            {
                return -1;
            }
            return (bArr[0] & 0xff) << 24
                        | (bArr[1] & 0xff) << 16
                        | (bArr[2] & 0xff) << 8
                        | (bArr[3] & 0xff) << 0;
        }

        /*
        public static string StringToHexArray(this string input)
        {
            char[] values = input.ToCharArray();
            StringBuilder sb = new StringBuilder(input.Length * 3);
            foreach (char letter in values)
            {
                // Get the integral value of the character.
                int value = Convert.ToInt32(letter);
                // Convert the decimal value to a hexadecimal value in string form.
                string hexOutput = string.Format("{0:X}", value);
                sb.Append(Convert.ToString(value, 16).PadLeft(2, '0').PadRight(3, ' '));
            }

            return sb.ToString().ToUpper();
        }
        */
    }
}
