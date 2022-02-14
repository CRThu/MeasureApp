using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MeasureApp.Model.Converter
{
    public static class BaseConverter
    {
        public static string BaseConverterInt64(string number, int fromBase, int toBase)
        {
            long int64Num = Convert.ToInt64(number, fromBase);
            return Convert.ToString(int64Num, toBase);
        }

        public static string BaseConverterInt64(long number, int toBase)
        {
            return Convert.ToString(number, toBase);
        }

        public static long BaseConverterInt64(string number, int fromBase)
        {
            return Convert.ToInt64(string.IsNullOrWhiteSpace(number) ? "0" : number, fromBase);
        }

        public static UInt16 BaseConverterUInt16(string number, int fromBase)
        {
            return Convert.ToUInt16(string.IsNullOrWhiteSpace(number) ? "0" : number, fromBase);
        }
    }
}
