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

        public bool IsSyncDCVDisplay = false;
        private ManualResetEvent resetEvent = new ManualResetEvent(false);
        public StringDataClass SyncDCVDisplayText = new StringDataClass() { StringData = "<null>" };
        public BoolDataBinding SyncDCVIsAutoStorage = new BoolDataBinding() { BoolData = false };
        public StringDataClass ManualReadDCVText = new StringDataClass() { StringData = "<null>" };
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

            SyncDCVDisplayTextBlock.DataContext = SyncDCVDisplayText;
            ManualReadDCVTextBlock.DataContext = ManualReadDCVText;
            SerialPortSendCmdPreviewTextBlock.DataContext = SerialPortSendCmdString;
            SyncDCVDisplayStorageCheckBox.DataContext = SyncDCVIsAutoStorage;

            SerialPortRecvDataTypesGrid.DataContext = serialPortRecvDataType;
            serialPortRecvDataType.SerialPortRecvDataTypeEnum = SerialPortRecvDataTypeEnum.Char;
            serialPortRecvDataType.SerialPortRecvDataEncodeEnum = SerialPortRecvDataEncodeEnum.Bytes;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            measure3458A.Dispose();
            serialPorts.CloseAll();
        }

        private void SyncDCVDisplayButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsSyncDCVDisplay)
            {
                IsSyncDCVDisplay = false;
                _ = resetEvent.Reset();
                SyncDCVDisplayButton.Content = "打开同步实时数据";
            }
            else
            {
                if (measure3458A.IsOpen)
                {
                    IsSyncDCVDisplay = true;
                    resetEvent.Set();
                    SyncDCVDisplayButton.Content = "关闭同步实时数据";
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            while (true)
                            {
                                _ = resetEvent.WaitOne();
                                // 3458A Multimeter User's Guide Page 149
                                decimal DCVDisplay = measure3458A.ReadDecimal();
                                SyncDCVDisplayText.StringData = $"{DCVDisplay}";
                                if (SyncDCVIsAutoStorage.BoolData)
                                {
                                    dataStorage.AddData(Key3458AString, DCVDisplay);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    });
                }
                else
                {
                    _ = MessageBox.Show("GPIB is not open.");
                }
            }
        }

        private void ManualReadDCVButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string setRange = (ManualReadDCVRangeComboBox.SelectedItem as ComboBoxItem).Tag as string;
                string setResolution = (ManualReadDCVResComboBox.SelectedItem as ComboBoxItem).Tag as string;
                decimal setRangeDecimal = Convert.ToDecimal(setRange);
                decimal setResolutionDecimal = Convert.ToDecimal(setResolution) / 1000000;

                ManualReadDCVText.StringData = "Measuring...";
                _ = Task.Run(() =>
                {
                    ManualReadDCVText.StringData = measure3458A.MeasureDCV(setRangeDecimal, setResolutionDecimal).ToString();
                });
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private void SetMinNPLCButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                measure3458A.SetNPLC(0);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private void SetMaxNPLCButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                measure3458A.SetNPLC(1000);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
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

        private void MultimeterStorageButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dataStorage.AddData(Key3458AString, Convert.ToDecimal(ManualReadDCVText.StringData));
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
