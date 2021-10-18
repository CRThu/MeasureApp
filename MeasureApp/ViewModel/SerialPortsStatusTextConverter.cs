using MeasureApp.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Windows.Data;

namespace MeasureApp
{
    public class SerialPortsStatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO MultiBinding
            //Select(serialPort => $"{serialPort.PortName}({serialPort.BaudRate}bps)").ToArray()) + "\nDevice Connected.";
            return value is Dictionary<string, SerialPort>.KeyCollection && (value as Dictionary<string, SerialPort>.KeyCollection).Count != 0
                ? string.Join(",\n", value as Dictionary<string, SerialPort>.KeyCollection) + "\nDevice Connected."
                : "No Device Connected.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
