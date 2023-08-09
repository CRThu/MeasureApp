using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Util
{
    public static class EscapeStringEx
    {
        public static string BytesToAsciiString(this byte[] bytes)
        {
            StringBuilder stringBuilder = new();
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == (byte)'\\')
                {
                    stringBuilder.Append(@"\\");
                }
                else if (bytes[i].IsPrintableAscii())
                {
                    stringBuilder.Append((char)bytes[i]);
                }
                else
                {
                    stringBuilder.Append($@"\{bytes[i].ToString("X2")}");
                }
            }
            return stringBuilder.ToString();
        }

        public static byte[] AsciiStringToBytes(this string asciiString)
        {
            int index = 0;
            List<byte> asciiChars = new();
            while (true)
            {
                if (index < asciiString.Length)
                {
                    asciiChars.Add((byte)asciiString[index++]);
                    if (asciiChars[^1] == '\\')
                    {
                        // 第一个为转义符
                        if (index < asciiString.Length)
                        {
                            asciiChars.RemoveAt(asciiChars.Count - 1);
                            asciiChars.Add((byte)asciiString[index++]);
                            if (asciiChars[^1] != '\\')
                            {
                                // 第二个为字符
                                if (index < asciiString.Length)
                                {
                                    asciiChars.Add((byte)asciiString[index++]);
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
    }
}
