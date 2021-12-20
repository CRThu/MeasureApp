using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.Common
{
    public static class BaseConverter
    {
        public static string BaseConverterInt64(string number, int fromBase, int toBase)
        {
            long int64Num = Convert.ToInt64(number, fromBase);
            return Convert.ToString(int64Num, toBase);
        }
    }
}
