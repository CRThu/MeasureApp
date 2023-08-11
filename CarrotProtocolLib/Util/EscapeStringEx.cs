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
                // 转义字符 '\' 转义为 '\'
                if (bytes[i] == (byte)'\\')
                {
                    stringBuilder.Append(@"\\");
                }
                // 转义字符 '\r' 转义为 '\r'
                else if (bytes[i] == (byte)'\r')
                {
                    stringBuilder.Append(@"\r");
                }
                // 转义字符 '\n' 转义为 '\n'
                else if (bytes[i] == (byte)'\n')
                {
                    stringBuilder.Append(@"\n");
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
            char currentChar;
            List<byte> asciiBytes = new();
            EscapeParseStatus status = EscapeParseStatus.LoadRawChar;
            while (index < escapeString.Length)
            {
                currentChar = escapeString[index];
                switch (status)
                {
                    case EscapeParseStatus.LoadRawChar:
                        // 非转义字符
                        if (currentChar != '\\')
                        {
                            status = EscapeParseStatus.LoadRawChar;
                            asciiBytes.Add((byte)currentChar);
                            index++;
                        }
                        // 当前字符为转义开始符号'\'
                        else
                        {
                            status = EscapeParseStatus.LoadEscapeChar0;
                            index++;
                        }
                        break;
                    case EscapeParseStatus.LoadEscapeChar0:
                        // 转义字符后的第一个字符为'\\'/'\r'/'\n', 即解析为'\'/'\r'/'\n',
                        if (currentChar == '\\')
                        {
                            status = EscapeParseStatus.LoadRawChar;
                            asciiBytes.Add((byte)'\\');
                            index++;
                        }
                        else if (currentChar == 'r')
                        {
                            status = EscapeParseStatus.LoadRawChar;
                            asciiBytes.Add((byte)'\r');
                            index++;
                        }
                        else if (currentChar == 'n')
                        {
                            status = EscapeParseStatus.LoadRawChar;
                            asciiBytes.Add((byte)'\n');
                            index++;
                        }
                        // 当前字符为字节,例如'\n[\*]' '\nn'
                        else
                        {
                            byte currNum = currentChar.HexCharToNum();
                            // 字符为非16进制字符则输出'\n'
                            if (currNum == 255)
                            {
                                status = EscapeParseStatus.LoadRawChar;
                                asciiBytes.Add((byte)'\\');
                                asciiBytes.Add((byte)currentChar);
                                index++;
                            }
                            // 字符为非16进制字符则输出 'n'对应的数字
                            else
                            {
                                status = EscapeParseStatus.LoadEscapeChar1;
                                asciiBytes.Add(currNum);
                                index++;
                            }
                        }
                        break;
                    case EscapeParseStatus.LoadEscapeChar1:
                        // 转义字符后的第二个字符为'\', 即'\n\', 解析为'\n'
                        if (currentChar == '\\')
                        {
                            status = EscapeParseStatus.LoadRawChar;
                        }
                        // 当前字符为字节,例如'\n[\*]' '\nn'
                        else
                        {
                            status = EscapeParseStatus.LoadRawChar;
                            byte currNum = currentChar.HexCharToNum();
                            // 字符为非16进制字符则输出'\n'
                            if (currNum == 255)
                            {
                                asciiBytes.Add((byte)currentChar);
                                index++;
                            }
                            // 字符为非16进制字符则输出 'nn'对应的数字
                            else
                            {
                                byte addByte = (byte)((asciiBytes[^1] << 4) + currNum);
                                asciiBytes[^1] = addByte;
                                index++;
                            }
                        }
                        break;
                }
            }
            return asciiBytes.ToArray();
        }

        public enum EscapeParseStatus
        {
            /// <summary>
            /// 已读取原始字符串
            /// </summary>
            LoadRawChar,
            /// <summary>
            /// 已读取转义字符 '\'
            /// </summary>
            LoadEscapeChar0,
            /// <summary>
            /// 已读取转义字符后的第一个字符
            /// </summary>
            LoadEscapeChar1,
        }
    }
}
