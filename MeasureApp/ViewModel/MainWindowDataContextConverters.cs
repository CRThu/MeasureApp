using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MeasureApp.ViewModel
{
    // 串口打开信息文本
    public class SerialPortsStatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Dictionary<string, SerialPort>.ValueCollection serialPortsRef = null;
            if (value is Dictionary<string, SerialPort>.ValueCollection)
                serialPortsRef = value as Dictionary<string, SerialPort>.ValueCollection;
            else
                throw new NotImplementedException();

            return serialPortsRef.Count != 0
                ? string.Join(", ", serialPortsRef.Select(ser => ser.PortName)) + " Device Connected."
                : "No Device Connected.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // 3458A 同步显示按钮文本
    public class M3458ASyncMeasureButtonContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "关闭同步测量值" : "开启同步测量值";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
