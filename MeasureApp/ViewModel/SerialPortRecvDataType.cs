using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;
using Newtonsoft.Json;

namespace MeasureApp
{
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

    public class SerialPortRecvDataType : NotificationObjectBase
    {
        // Ascii, Bytes RadioButton Binding
        private SerialPortRecvDataEncodeEnum serialPortRecvDataEncodeEnum;
        public SerialPortRecvDataEncodeEnum SerialPortRecvDataEncodeEnum
        {
            get => serialPortRecvDataEncodeEnum;
            set
            {
                serialPortRecvDataEncodeEnum = value;
                RaisePropertyChanged(() => SerialPortRecvDataEncodeEnum);
            }
        }

        // Char, Int8, Int16, Int32 RadioButton Binding
        private SerialPortRecvDataTypeEnum serialPortRecvDataTypeEnum;
        public SerialPortRecvDataTypeEnum SerialPortRecvDataTypeEnum
        {
            get => serialPortRecvDataTypeEnum;
            set
            {
                serialPortRecvDataTypeEnum = value;
                RaisePropertyChanged(() => SerialPortRecvDataTypeEnum);
            }
        }

        // Array<T> CheckBox Binding
        private bool isArray;
        public bool IsArray
        {
            get => isArray;
            set
            {
                isArray = value;
                RaisePropertyChanged(() => IsArray);
            }
        }

        // Array<T>.Count TextBox Binding
        private int arrayCount;
        public int ArrayCount
        {
            get => arrayCount;
            set
            {
                arrayCount = value;
                RaisePropertyChanged(() => ArrayCount);
            }
        }

        public int RequiredBytesLength
        {
            get
            {
                int byteLength;
                switch (serialPortRecvDataEncodeEnum)
                {
                    case SerialPortRecvDataEncodeEnum.Ascii:
                        return 0;
                    case SerialPortRecvDataEncodeEnum.Bytes:
                        switch (serialPortRecvDataTypeEnum)
                        {
                            case SerialPortRecvDataTypeEnum.Char:
                                byteLength = System.Runtime.InteropServices.Marshal.SizeOf(typeof(char));
                                break;
                            case SerialPortRecvDataTypeEnum.UInt8:
                                byteLength = System.Runtime.InteropServices.Marshal.SizeOf(typeof(byte));
                                break;
                            case SerialPortRecvDataTypeEnum.UInt16:
                                byteLength = System.Runtime.InteropServices.Marshal.SizeOf(typeof(UInt16));
                                break;
                            case SerialPortRecvDataTypeEnum.UInt32:
                                byteLength = System.Runtime.InteropServices.Marshal.SizeOf(typeof(UInt32));
                                break;
                            default:
                                return 0;
                        }
                        break;
                    default:
                        return 0;
                }
                return isArray ? byteLength * ArrayCount : byteLength;
            }
        }

        public static T[] FromBytes<T>(byte[] bytes) where T : struct
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("Bytes");
            }
            T[] array = new T[bytes.Length / System.Runtime.InteropServices.Marshal.SizeOf(typeof(T))];
            Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
            return array;
        }

        public static T ConvertObject<T>(object asObject) where T:new()
        {
            string json = JsonConvert.SerializeObject(asObject);
            T t = JsonConvert.DeserializeObject<T>(json);
            return t;
        }
    }
}
