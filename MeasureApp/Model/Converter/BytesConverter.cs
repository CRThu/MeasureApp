using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.Converter
{
    public static class BytesConverter
    {
        // "AABBCCDDEEFF" => {0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF}
        public static byte[] Hex2Bytes(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if (hexString.Length % 2 != 0)
            {
                hexString += " ";
            }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }

        // {0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF} => "AABBCCDDEEFF"
        public static string Bytes2Hex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        // "ABC" => {0x41, 0x42, 0x43}
        public static byte[] String2Bytes(string str)
        {
            return System.Text.Encoding.ASCII.GetBytes(str);
        }

        // {0x41, 0x42, 0x43} => "ABC"
        public static string Bytes2String(byte[] bytes)
        {
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        public static T[] FromBytes<T>(byte[] bytes) where T : struct
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("Bytes");
            }
            T[] array = new T[bytes.Length / System.Runtime.InteropServices.Marshal.SizeOf(typeof(T))];
            Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
            return array;
        }

        public static T[] FromBytes<T>(string bytes) where T : struct
        {
            return FromBytes<T>(Hex2Bytes(bytes));
        }
    }
}
