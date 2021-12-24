using LiveCharts.Geared;
using MeasureApp.Model;
using MeasureApp.Model.Converter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

    // 串口命令监听按钮文本
    public class SerialPortCommandIsListeningButtonContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "关闭监听" : "打开监听";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // StatusBar 秒数转时间转换器
    public class SecondsToTimeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new TimeSpan(0, 0, (int)value).ToString(@"hh\:mm\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    // 自动化任务运行/停止按钮文本
    public class AutomationTaskRunButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "停止" : "运行";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // 串口数据模块转换器与Enum

    public enum SerialPortRecvDataTypeEnum
    {
        Char,
        UInt8,
        UInt16,
        UInt32
    }

    public enum SerialPortRecvDataEncodeEnum
    {
        Ascii,
        Bytes
    }

    public class SerialPortRecvDataTypeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SerialPortRecvDataTypeEnum serialPortRecvDataType = (SerialPortRecvDataTypeEnum)value;
            return serialPortRecvDataType == (SerialPortRecvDataTypeEnum)int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = (bool)value;
            return !isChecked ? null : (object)(SerialPortRecvDataTypeEnum)int.Parse(parameter.ToString());
        }
    }

    public class SerialPortRecvDataEncodeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SerialPortRecvDataEncodeEnum serialPortRecvDataEncode = (SerialPortRecvDataEncodeEnum)value;
            return serialPortRecvDataEncode == (SerialPortRecvDataEncodeEnum)int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = (bool)value;
            return !isChecked ? null : (object)(SerialPortRecvDataEncodeEnum)int.Parse(parameter.ToString());
        }
    }

    // bool转控件Visiblilty属性
    // Visibility="{Binding ElementName=SerialPortSendCmdIsHexCheckBox, Path=IsChecked, Converter={StaticResource BoolInverter2ControlVisibilityConverter}}"
    public class Bool2ControlVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolInverter2ControlVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime)value).ToString("yyyy-MM-dd hh:mm:ss.fff");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsNotNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Num2HexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return BaseConverter.BaseConverterInt64((uint)value, 16);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return BaseConverter.BaseConverterInt64((string)value, 16);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return 0x0;
            }
        }
    }

    public class String2NullableDecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal? d = (decimal?)value;
            if (d.HasValue)
                return d.Value.ToString(culture);
            else
                return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = (string)value;
            if (string.IsNullOrEmpty(s))
                return null;
            else
                return (decimal?)decimal.Parse(s, culture);
        }
    }

}
