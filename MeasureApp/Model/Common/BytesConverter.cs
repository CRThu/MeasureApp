using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.Common
{
    public static class BytesConverter
    {
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

        public static string Bytes2Hex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}
