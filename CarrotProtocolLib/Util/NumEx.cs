using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Util
{
    public static class NumEx
    {
        /// <summary>
        /// 解析成数字
        /// "123" => 123
        /// "0x123" => 291
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int ParseNum(this string s)
        {
            try
            {
                if (s.StartsWith("0x"))
                {
                    return Convert.ToInt32(s[2..], 16);
                }
                else if (s.StartsWith("0b"))
                {
                    return Convert.ToInt32(s[2..], 2);
                }
                else
                {
                    return Convert.ToInt32(s);
                }
            }
            catch
            {
                // null or error
                return 0;
            }
        }
    }
}
