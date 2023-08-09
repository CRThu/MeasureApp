using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Util
{
    /// <summary>
    /// 转义字符串拓展类
    /// </summary>
    public static class EscapeStringEx
    {
        /// <summary>
        /// 字节数组转转义字符串
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns>转义字符串</returns>
        public static string BytesToEscapeString(this byte[] bytes)
        {
            StringBuilder stringBuilder = new();
            for (int i = 0; i < bytes.Length; i++)
            {
                // 转义字符 '\' 转义为 '\\'
                if (bytes[i] == (byte)'\\')
                {
                    stringBuilder.Append(@"\\");
                }
                // 可打印字符串不转义
                else if (bytes[i].IsPrintableAscii())
                {
                    stringBuilder.Append((char)bytes[i]);
                }
                // 不可打印字符串转义为 '\NN'
                else
                {
                    stringBuilder.Append($@"\{bytes[i].ToString("X2")}");
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 转义字符串转字节数组
        /// </summary>
        /// <param name="escapeString">转义字符串</param>
        /// <returns></returns>
        public static byte[] EscapeStringToBytes(this string escapeString)
        {
            int index = 0;
            List<byte> asciiChars = new();
            while (true)
            {
                if (index < escapeString.Length)
                {
                    asciiChars.Add((byte)escapeString[index++]);
                    if (asciiChars[^1] == '\\')
                    {
                        // 第一个为转义符
                        if (index < escapeString.Length)
                        {
                            asciiChars.RemoveAt(asciiChars.Count - 1);
                            asciiChars.Add((byte)escapeString[index++]);
                            if (asciiChars[^1] != '\\')
                            {
                                // 第二个为字符
                                if (index < escapeString.Length)
                                {
                                    asciiChars.Add((byte)escapeString[index++]);
                                    string byteStr = ((char)asciiChars[^2]).ToString() + ((char)asciiChars[^1]).ToString();

                                    asciiChars.RemoveAt(asciiChars.Count - 1);
                                    asciiChars[asciiChars.Count - 1] = Convert.ToByte(byteStr, 16);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            return asciiChars.ToArray();
        }

        public enum EscapeParseFsmStatus
        {

        }
    }
}
