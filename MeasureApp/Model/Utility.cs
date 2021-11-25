using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MeasureApp.Model
{
    public static class Utility
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

        public static T TimeoutCheck<T>(int millisecondsTimeout, Func<T> func)
        {
            ManualResetEvent resetEvent = new(false);
            bool isCompleted = false;
            Task<T> task = Task.Run(() =>
            {
                T result = func.Invoke();
                isCompleted = true;
                resetEvent.Set();
                return result;
            });
            _ = resetEvent.WaitOne(millisecondsTimeout);
            return isCompleted ? task.Result : default;
        }
    }
}
