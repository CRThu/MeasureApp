using System;
using System.Collections.Generic;
using System.Globalization;

namespace MeasureApp
{
    public class SerialPortsStatusTextConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO
            //Select(serialPort => $"{serialPort.PortName}({serialPort.BaudRate}bps)").ToArray()) + "\nDevice Connected.";
            // return string.Join(",\n", value as Dictionary<string, dynamic>.KeyCollection);
            return "TEST";
        }
    }
}
