using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CarrotProtocolCommDemo.Converter
{
    public class DictionaryItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is not null && values.Length >= 2)
            {
                var myDict = values[0] as IDictionary;
                var myKey = values[1] as string;
                if (myDict != null && myKey != null)
                {
                    if (myDict.Contains(myKey))
                        return myDict[myKey]!;
                }
            }
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
