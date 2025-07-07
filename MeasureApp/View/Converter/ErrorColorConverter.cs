using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MeasureApp.View.Converter
{
    /// <summary>
    /// Binding BooleanObject,
    /// Converter={StaticResource ErrorColorConverter},
    /// ConverterParameter='TrueString|FalseString'
    /// </summary>
    public class ErrorColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Brushes.Red : Brushes.Green;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
