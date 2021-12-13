using MeasureApp.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 串口指令发送模块文本框前景色绑定
        private SolidColorBrush serialPortSendCmdCommandNameManualTextBoxForeGround = new(Colors.Black);
        public SolidColorBrush SerialPortSendCmdCommandNameManualTextBoxForeGround
        {
            get => serialPortSendCmdCommandNameManualTextBoxForeGround;
            set
            {
                serialPortSendCmdCommandNameManualTextBoxForeGround = value;
                RaisePropertyChanged(() => SerialPortSendCmdCommandNameManualTextBoxForeGround);
            }
        }

        private SolidColorBrush serialPortSendCmdParamsTextBoxForeGround = new(Colors.Black);
        public SolidColorBrush SerialPortSendCmdParamsTextBoxForeGround
        {
            get => serialPortSendCmdParamsTextBoxForeGround;
            set
            {
                serialPortSendCmdParamsTextBoxForeGround = value;
                RaisePropertyChanged(() => SerialPortSendCmdParamsTextBoxForeGround);
            }
        }

        // 串口指令发送模块串口选择数据绑定
        private string serialPortSendCmdSerialPortNameSelectedValue;
        public string SerialPortSendCmdSerialPortNameSelectedValue
        {
            get => serialPortSendCmdSerialPortNameSelectedValue;
            set
            {
                serialPortSendCmdSerialPortNameSelectedValue = value;
                RaisePropertyChanged(() => SerialPortSendCmdSerialPortNameSelectedValue);
            }
        }

        // 串口指令发送模块命令选择数据绑定
        private string serialportSendCmdCommandNameSelectedValue = "SET";
        public string SerialportSendCmdCommandNameSelectedValue
        {
            get => serialportSendCmdCommandNameSelectedValue;
            set
            {
                serialportSendCmdCommandNameSelectedValue = value;
                RaisePropertyChanged(() => SerialportSendCmdCommandNameSelectedValue);

            }
        }

        private string serialPortSendCmdCommandNameManualText = "MANUAL";
        public string SerialPortSendCmdCommandNameManualText
        {
            get => serialPortSendCmdCommandNameManualText;
            set
            {
                serialPortSendCmdCommandNameManualText = value;
                RaisePropertyChanged(() => SerialPortSendCmdCommandNameManualText);
            }
        }

        // 串口指令发送模块参数16进制
        private bool serialPortSendCmdIsHex;
        public bool SerialPortSendCmdIsHex
        {
            get => serialPortSendCmdIsHex;
            set
            {
                serialPortSendCmdIsHex = value;
                RaisePropertyChanged(() => SerialPortSendCmdIsHex);
            }
        }

        // 串口指令发送模块参数16进制
        private string serialPortSendCmdHexText;
        public string SerialPortSendCmdHexText
        {
            get => serialPortSendCmdHexText;
            set
            {
                serialPortSendCmdHexText = value;
                RaisePropertyChanged(() => SerialPortSendCmdHexText);
            }
        }

        // 串口指令发送模块参数文本框数据绑定
        private string serialPortSendCmdParamsText;
        public string SerialPortSendCmdParamsText
        {
            get => serialPortSendCmdParamsText;
            set
            {
                serialPortSendCmdParamsText = value;
                RaisePropertyChanged(() => SerialPortSendCmdParamsText);
            }
        }

        // 串口指令发送模块参数文本框数据绑定
        private string serialPortSendCmdPreviewText;
        public string SerialPortSendCmdPreviewText
        {
            get => serialPortSendCmdPreviewText;
            set
            {
                serialPortSendCmdPreviewText = value;
                RaisePropertyChanged(() => SerialPortSendCmdPreviewText);
            }
        }

        // 接收数据显示文本框绑定
        private string serialPortRecvDataDisplayText;
        public string SerialPortRecvDataDisplayText
        {
            get => serialPortRecvDataDisplayText;
            set
            {
                serialPortRecvDataDisplayText = value;
                RaisePropertyChanged(() => SerialPortRecvDataDisplayText);
            }
        }

        // Ascii, Bytes RadioButton Binding
        private SerialPortRecvDataEncodeEnum serialPortRecvDataEncodeEnum = SerialPortRecvDataEncodeEnum.Bytes;
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
        private SerialPortRecvDataTypeEnum serialPortRecvDataTypeEnum = SerialPortRecvDataTypeEnum.Char;
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

        public static T ConvertObject<T>(object asObject) where T : new()
        {
            string json = JsonConvert.SerializeObject(asObject);
            T t = JsonConvert.DeserializeObject<T>(json);
            return t;
        }

        // 串口指令发送模块命令编辑事件
        private CommandBase serialPortSendCmdChangedEvent;
        public CommandBase SerialPortSendCmdChangedEvent
        {
            get
            {
                if (serialPortSendCmdChangedEvent == null)
                {
                    serialPortSendCmdChangedEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            string commandName = SerialportSendCmdCommandNameSelectedValue == "Others" ? SerialPortSendCmdCommandNameManualText : SerialportSendCmdCommandNameSelectedValue;
                            string[] CommandElements = (commandName + ";" + SerialPortSendCmdParamsText ?? "").Split(" ,;|&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            SerialPortSendCmdPreviewText = $"{string.Join(";", CommandElements)};";

                            SerialPortSendCmdCommandNameManualTextBoxForeGround = new SolidColorBrush(Regex.IsMatch(SerialPortSendCmdCommandNameManualText ?? "", @"[^a-zA-Z0-9.]") ? Colors.Red : Colors.Black);
                            SerialPortSendCmdParamsTextBoxForeGround = new SolidColorBrush(Regex.IsMatch(SerialPortSendCmdParamsText ?? "", @"[^x00-xff.\s,;|&]") ? Colors.Red : Colors.Black);
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return serialPortSendCmdChangedEvent;
            }
        }

        // 串口指令发送模块发送事件
        private CommandBase serialPortSendCmdEvent;
        public CommandBase SerialPortSendCmdEvent
        {
            get
            {
                if (serialPortSendCmdEvent == null)
                {
                    serialPortSendCmdEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            if(SerialPortSendCmdIsHex)
                            {
                                SerialPortsInstance.WriteBytes(SerialPortSendCmdSerialPortNameSelectedValue, Utility.Hex2Bytes(SerialPortSendCmdHexText));
                            }
                            else
                            {
                                SerialPortsInstance.WriteString(SerialPortSendCmdSerialPortNameSelectedValue, SerialPortSendCmdPreviewText);
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return serialPortSendCmdEvent;
            }
        }

        // 串口数据接收模块串口选择数据绑定
        private string serialPortRecvDataSerialPortNameSelectedValue;
        public string SerialPortRecvDataSerialPortNameSelectedValue
        {
            get => serialPortRecvDataSerialPortNameSelectedValue;
            set
            {
                serialPortRecvDataSerialPortNameSelectedValue = value;
                RaisePropertyChanged(() => SerialPortRecvDataSerialPortNameSelectedValue);
            }
        }


        // 串口接收数据事件
        // TODO 优化此代码
        public dynamic RecvDataPraseTemp;
        private CommandBase serialPortRecvDataEvent;
        public CommandBase SerialPortRecvDataEvent
        {
            get
            {
                if (serialPortRecvDataEvent == null)
                {
                    serialPortRecvDataEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            string serialPortName = SerialPortRecvDataSerialPortNameSelectedValue;
                            switch (SerialPortRecvDataEncodeEnum)
                            {
                                case SerialPortRecvDataEncodeEnum.Ascii:
                                    string recvString = SerialPortsInstance.ReadExistingString(serialPortName);
                                    switch (SerialPortRecvDataTypeEnum)
                                    {
                                        case SerialPortRecvDataTypeEnum.Char:
                                            RecvDataPraseTemp = recvString;
                                            break;
                                        case SerialPortRecvDataTypeEnum.UInt8:
                                            RecvDataPraseTemp = recvString.Split(" ,;|&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(data => Convert.ToByte(data)).ToArray();
                                            break;
                                        case SerialPortRecvDataTypeEnum.UInt16:
                                            RecvDataPraseTemp = recvString.Split(" ,;|&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(data => Convert.ToUInt16(data)).ToArray();
                                            break;
                                        case SerialPortRecvDataTypeEnum.UInt32:
                                            RecvDataPraseTemp = recvString.Split(" ,;|&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(data => Convert.ToInt32(data)).ToArray();
                                            break;
                                        default:
                                            throw new NotImplementedException();
                                    }
                                    break;
                                case SerialPortRecvDataEncodeEnum.Bytes:
                                    int bytesLength = RequiredBytesLength;
                                    byte[] recvBytes = new byte[bytesLength];
                                    int recvBytesLen = SerialPortsInstance.ReadBytesWithTimeoutCheck(serialPortName, recvBytes, bytesLength);
                                    if (recvBytesLen != recvBytes.Length)
                                    {
                                        _ = MessageBox.Show($"读取超时, RecvLength = {recvBytesLen}/{recvBytes.Length}!");
                                        SerialPortRecvDataDisplayText = Utility.Bytes2Hex(recvBytes);
                                        return;
                                    }
                                    switch (SerialPortRecvDataTypeEnum)
                                    {
                                        case SerialPortRecvDataTypeEnum.Char:
                                            RecvDataPraseTemp = Encoding.ASCII.GetString(recvBytes);
                                            break;
                                        case SerialPortRecvDataTypeEnum.UInt8:
                                            RecvDataPraseTemp = recvBytes;
                                            break;
                                        case SerialPortRecvDataTypeEnum.UInt16:
                                            RecvDataPraseTemp = FromBytes<UInt16>(recvBytes);
                                            break;
                                        case SerialPortRecvDataTypeEnum.UInt32:
                                            RecvDataPraseTemp = FromBytes<UInt32>(recvBytes);
                                            break;
                                        default:
                                            throw new NotImplementedException();
                                    }
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                            SerialPortRecvDataDisplayText = RecvDataPraseTemp is Array ? string.Join(", ", RecvDataPraseTemp) : RecvDataPraseTemp.ToString();
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return serialPortRecvDataEvent;
            }
        }



        // 串口接收数据事件
        private CommandBase serialPortRecvDataStorageEvent;
        public CommandBase SerialPortRecvDataStorageEvent
        {
            get
            {
                if (serialPortRecvDataStorageEvent == null)
                {
                    serialPortRecvDataStorageEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            if (RecvDataPraseTemp is Array)
                            {
                                object[] objArray = new object[RecvDataPraseTemp.Length];
                                RecvDataPraseTemp.CopyTo(objArray, 0);
                                foreach (object obj in objArray)
                                {
                                    DataStorageInstance.DataStorageDictionary[KeySerialPortString].Add(new ObservableValue { Value = obj });
                                }
                            }
                            else
                            {
                                DataStorageInstance.DataStorageDictionary[KeySerialPortString].Add(new ObservableValue { Value = RecvDataPraseTemp });
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return serialPortRecvDataStorageEvent;
            }
        }
    }
}
