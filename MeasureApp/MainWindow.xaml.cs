using MeasureApp.Model;
using MeasureApp.ViewModel;
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
        GPIB3458AMeasure measure3458A;
        DataStorage dataStorage;
        SerialPorts serialPorts;
        string Key3458AString = "3458A Data Storage";
        string KeySerialPortString = "Serial Port Data Storage";

        public StringDataClass SerialPortSendCmdString = new StringDataClass() { StringData = "<null>::<null>;" };

        public SerialPortRecvDataType serialPortRecvDataType = new SerialPortRecvDataType();
        private dynamic RecvDataPraseTemp;

        public MainWindow()
        {
            // 重构临时变量
            measure3458A = mainWindowDataContext.Measure3458AInstance;
            dataStorage = mainWindowDataContext.DataStorageInstance;
            serialPorts = mainWindowDataContext.SerialPortsInstance;

            InitializeComponent();

            //DataContext = new MainWindowDataContext();
            DataContext = mainWindowDataContext;

            mainWindowDataContext.GpibDeviceSearchEvent.Execute(null);
            mainWindowDataContext.SerialPortDeviceSearchEvent.Execute(null);
            SerialPortSendCmd_Changed(null, null);

            // TODO
            SerialPortSendCmdPreviewTextBlock.DataContext = SerialPortSendCmdString;

            SerialPortRecvDataTypesGrid.DataContext = serialPortRecvDataType;
            serialPortRecvDataType.SerialPortRecvDataTypeEnum = SerialPortRecvDataTypeEnum.Char;
            serialPortRecvDataType.SerialPortRecvDataEncodeEnum = SerialPortRecvDataEncodeEnum.Bytes;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            measure3458A.Dispose();
            serialPorts.CloseAll();
        }

        private void SerialPortWriteCmdButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                serialPorts.WriteString(SerialPortDebugSelectComboBox.SelectedItem as string, SerialPortWriteCmdTextBox.Text);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private void SerialPortReadCmdButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SerialPortReadCmdTextBox.Text = serialPorts.ReadExistingString(SerialPortDebugSelectComboBox.SelectedItem as string);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private void SerialPortSendCmd_Changed(object sender, object e)
        {
            try
            {
                if (SerialPortSendCmdSerialPortNameComboBox != null
                && SerialPortSendCmdCommandNameComboBox != null
                && SerialPortSendCmdParamsTextBox != null)
                {
                    string serialPortName = SerialPortSendCmdSerialPortNameComboBox.SelectedItem as string;
                    string commandName = ((SerialPortSendCmdCommandNameComboBox.SelectedItem as ComboBoxItem).Tag as string) == "Other"
                            ? ((SerialPortSendCmdCommandNameComboBox.SelectedItem as ComboBoxItem).Content as TextBox).Text
                            : (SerialPortSendCmdCommandNameComboBox.SelectedItem as ComboBoxItem).Tag as string;
                    string[] CommandElements = (commandName + ";" + SerialPortSendCmdParamsTextBox.Text).Split(" ,.;|&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    if (((SerialPortSendCmdCommandNameComboBox.SelectedItem as ComboBoxItem).Tag as string) == "Other")
                        ((SerialPortSendCmdCommandNameComboBox.SelectedItem as ComboBoxItem).Content as TextBox).Foreground = new SolidColorBrush(Regex.IsMatch(((SerialPortSendCmdCommandNameComboBox.SelectedItem as ComboBoxItem).Content as TextBox).Text, @"[^a-zA-Z0-9]") ? Colors.Red : Colors.Black);
                    SerialPortSendCmdParamsTextBox.Foreground = new SolidColorBrush(Regex.IsMatch(SerialPortSendCmdParamsTextBox.Text, @"[^x00-xff\s,.;|&]") ? Colors.Red : Colors.Black);
                    SerialPortSendCmdString.StringData = $"{serialPortName}::{string.Join(";", CommandElements)};";
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private void SerialPortSendCmdButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] splitCmds = SerialPortSendCmdString.StringData.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                serialPorts.WriteString(splitCmds[0], splitCmds[1]);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private void SerialPortRecvDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string serialPortName = SerialPortRecvDataSerialPortNameComboBox.SelectedItem as string;
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
