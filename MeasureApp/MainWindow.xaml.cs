using MeasureApp.Model;
using MeasureApp.ViewModel;
using Microsoft.Xaml.Behaviors;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MeasureApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowDataContext mainWindowDataContext = new MainWindowDataContext();

        // 重构临时变量
        DataStorage dataStorage;
        SerialPorts serialPorts;
        string KeySerialPortString = "Serial Port Data Storage";

        public SerialPortRecvDataType serialPortRecvDataType = new SerialPortRecvDataType();
        private dynamic RecvDataPraseTemp;

        public MainWindow()
        {
            // 重构临时变量
            dataStorage = mainWindowDataContext.DataStorageInstance;
            serialPorts = mainWindowDataContext.SerialPortsInstance;

            InitializeComponent();

            //DataContext = new MainWindowDataContext();
            DataContext = mainWindowDataContext;

            // TEST
            //Button btn = new Button();
            //btn.Content = "Button";
            //var triggers = Interaction.GetTriggers(btn);
            //Microsoft.Xaml.Behaviors.EventTrigger trigger1 = new("Click");
            //trigger1.Actions.Add(new EventCommandBase() { Command = TestEvent });
            //triggers.Add(trigger1);
            //SettingsGrid.Children.Add(btn);

            // TODO
            SerialPortRecvDataTypesGrid.DataContext = serialPortRecvDataType;
            serialPortRecvDataType.SerialPortRecvDataTypeEnum = SerialPortRecvDataTypeEnum.Char;
            serialPortRecvDataType.SerialPortRecvDataEncodeEnum = SerialPortRecvDataEncodeEnum.Bytes;
        }

        private void SerialPortRecvDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string serialPortName = mainWindowDataContext.SerialPortRecvDataSerialPortNameSelectedValue;
                switch (serialPortRecvDataType.SerialPortRecvDataEncodeEnum)
                {
                    case SerialPortRecvDataEncodeEnum.Ascii:
                        string recvString = serialPorts.ReadExistingString(serialPortName);
                        switch (serialPortRecvDataType.SerialPortRecvDataTypeEnum)
                        {
                            case SerialPortRecvDataTypeEnum.Char:
                                RecvDataPraseTemp = recvString;
                                break;
                            case SerialPortRecvDataTypeEnum.UInt8:
                                RecvDataPraseTemp = recvString.Split(" ,.;|&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(data => Convert.ToByte(data)).ToArray();
                                break;
                            case SerialPortRecvDataTypeEnum.UInt16:
                                RecvDataPraseTemp = recvString.Split(" ,.;|&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(data => Convert.ToUInt16(data)).ToArray();
                                break;
                            case SerialPortRecvDataTypeEnum.UInt32:
                                RecvDataPraseTemp = recvString.Split(" ,.;|&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(data => Convert.ToInt32(data)).ToArray();
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        break;
                    case SerialPortRecvDataEncodeEnum.Bytes:
                        int bytesLength = serialPortRecvDataType.RequiredBytesLength;
                        byte[] recvBytes = new byte[bytesLength];
                        int recvBytesLen = serialPorts.Read(serialPortName, recvBytes, bytesLength);
                        if (recvBytesLen != recvBytes.Length)
                        {
                            _ = MessageBox.Show($"读取超时, RecvLength = {recvBytesLen}/{recvBytes.Length}!");
                            SerialPortRecvDataDisplayTextBox.Text = BitConverter.ToString(recvBytes);
                            return;
                        }
                        switch (serialPortRecvDataType.SerialPortRecvDataTypeEnum)
                        {
                            case SerialPortRecvDataTypeEnum.Char:
                                RecvDataPraseTemp = Encoding.ASCII.GetString(recvBytes);
                                break;
                            case SerialPortRecvDataTypeEnum.UInt8:
                                RecvDataPraseTemp = recvBytes;
                                break;
                            case SerialPortRecvDataTypeEnum.UInt16:
                                RecvDataPraseTemp = SerialPortRecvDataType.FromBytes<UInt16>(recvBytes);
                                break;
                            case SerialPortRecvDataTypeEnum.UInt32:
                                RecvDataPraseTemp = SerialPortRecvDataType.FromBytes<UInt32>(recvBytes);
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
                SerialPortRecvDataDisplayTextBox.Text = RecvDataPraseTemp is Array ? string.Join(", ", RecvDataPraseTemp) : RecvDataPraseTemp.ToString();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private void SerialPortRecvDataStorageButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RecvDataPraseTemp is Array)
                {
                    object[] objArray = new object[RecvDataPraseTemp.Length];
                    RecvDataPraseTemp.CopyTo(objArray, 0);
                    foreach (object obj in objArray)
                    {
                        dataStorage.DataStorageDictionary[KeySerialPortString].Add(new StringDataClass { StringData = obj.ToString() });
                    }
                }
                else
                {
                    dataStorage.DataStorageDictionary[KeySerialPortString].Add(new StringDataClass { StringData = RecvDataPraseTemp.ToString() });
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }
    }
}
