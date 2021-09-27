using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MeasureApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public GPIB gpib = new GPIB();
        public SerialPorts serialPorts = new SerialPorts();

        public ObservableCollection<string> gpibDeviceNames = new ObservableCollection<string>();

        public bool IsSyncDCVDisplay = false;
        private ManualResetEvent resetEvent = new ManualResetEvent(false);
        public StringDataBinding SyncDCVDisplayText = new StringDataBinding() { StringData = "<null>" };
        public BoolDataBinding SyncDCVIsAutoStorage = new BoolDataBinding() { BoolData = false };
        public StringDataBinding ManualReadDCVText = new StringDataBinding() { StringData = "<null>" };
        public StringDataBinding SerialPortSendCmdString = new StringDataBinding() { StringData = "<null>::<null>;" };

        public SerialPortRecvDataType serialPortRecvDataType = new SerialPortRecvDataType();
        private dynamic RecvDataPraseTemp;
        //public List<dynamic> serialPortRecvDataStorage = new List<dynamic>();
        public ObservableCollection<StringDataBinding> MultimeterDataStorage = new ObservableCollection<StringDataBinding>();
        public ObservableCollection<StringDataBinding> SerialPortRecvDataStorage = new ObservableCollection<StringDataBinding>();

        public MainWindow()
        {
            InitializeComponent();

            SearchGPIBDevicesButton_Click(null, null);
            SearchSerialPortButton_Click(null, null);
            SerialPortSendCmd_Changed(null, null);

            DeviceComboBox.ItemsSource = gpibDeviceNames;
            SyncDCVDisplayTextBlock.DataContext = SyncDCVDisplayText;
            ManualReadDCVTextBlock.DataContext = ManualReadDCVText;
            SerialPortSendCmdPreviewTextBlock.DataContext = SerialPortSendCmdString;
            SyncDCVDisplayStorageCheckBox.DataContext = SyncDCVIsAutoStorage;
            SerialPortRecvDataStorageDataGrid.ItemsSource = SerialPortRecvDataStorage;

            SerialPortRecvDataTypesGrid.DataContext = serialPortRecvDataType;
            serialPortRecvDataType.SerialPortRecvDataTypeEnum = SerialPortRecvDataTypeEnum.Char;
            serialPortRecvDataType.SerialPortRecvDataEncodeEnum = SerialPortRecvDataEncodeEnum.Bytes;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            gpib.Dispose();
            serialPorts.CloseAll();
        }

        private void GeneralDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void SearchGPIBDevicesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                gpibDeviceNames.Clear();
                foreach (string resource in GPIB.SearchDevices("GPIB?*INSTR"))
                {
                    gpibDeviceNames.Add(resource);
                }
            }
            catch (Exception ex)
            {
                // _ = MessageBox.Show(ex.ToString());
            }

            if (gpibDeviceNames.Count != 0 && DeviceComboBox.SelectedIndex == -1)
            {
                DeviceComboBox.SelectedIndex = 0;
            }
        }

        private void OpenGPIBDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                gpib.Dispose();
                gpib.Open(DeviceComboBox.SelectedItem as string);
                gpib.messageBasedSession.Timeout = Properties.Settings.Default.GPIBTimeout;
                gpib.Write("END");
                string deviceName = gpib.Query("ID?");
                DeviceNameLabel.Text = deviceName;
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private void QueryCmdButton_Click(object sender, RoutedEventArgs e)
        {
            if (gpib.IsOpen)
            {
                try
                {
                    ReadCmdTextBox.Text = gpib.Query(WriteCmdTextBox.Text).ToString();
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                _ = MessageBox.Show("GPIB is not open.");
            }
        }

        private void WriteCmdButton_Click(object sender, RoutedEventArgs e)
        {
            if (gpib.IsOpen)
            {
                try
                {
                    gpib.Write(WriteCmdTextBox.Text);
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                _ = MessageBox.Show("GPIB is not open.");
            }
        }

        private void ReadCmdButton_Click(object sender, RoutedEventArgs e)
        {
            if (gpib.IsOpen)
            {
                try
                {
                    ReadCmdTextBox.Text = gpib.ReadString();

                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                _ = MessageBox.Show("GPIB is not open.");
            }
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
                if (gpib.IsOpen)
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
                                decimal DCVDisplay = gpib.ReadDemical();
                                SyncDCVDisplayText.StringData = $"{DCVDisplay}";
                                if (SyncDCVIsAutoStorage.BoolData)
                                {
                                    // CollectionView不支持从调度程序以外的线程对其SourceCollection进行的更改
                                    SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Application.Current.Dispatcher));
                                    SynchronizationContext.Current.Post(p1 =>
                                    {
                                        MultimeterDataStorage.Add(new StringDataBinding() { StringData = DCVDisplay.ToString() });
                                    }, null);
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

        private void BasicGuiConfigButtons_Click(object sender, RoutedEventArgs e)
        {
            if (gpib.IsOpen)
            {
                try
                {
                    switch ((sender as Button).Tag as string)
                    {
                        case "RESET":
                            gpib.Write("RESET");
                            gpib.Write("END");
                            gpib.Write("NDIG 8");
                            GuiConfigLogTextBox.Text = $"Write: RESET & END & NDIG 8";
                            break;
                        case "ID":
                            GuiConfigLogTextBox.Text = $"Query: ID?\nReturn: {gpib.Query("ID?")}";
                            break;
                        case "ERR":
                            GuiConfigLogTextBox.Text = $"Query: ERRSTR?\nReturn: {gpib.Query("ERRSTR?")}";
                            break;
                        case "STB":
                            GuiConfigLogTextBox.Text = $"Query: STB?\nReturn: {gpib.Query("STB?")}";
                            break;
                        case "TEMP":
                            GuiConfigLogTextBox.Text = $"Query: TEMP?\nReturn: {gpib.Query("TEMP?")}";
                            break;
                        case "LINE":
                            GuiConfigLogTextBox.Text = $"Query: LINE?\nReturn: {gpib.QueryDemical("LINE?")} Hz";
                            break;
                        case "NDIG":
                            string setNdig = (NdigComboBox.SelectedItem as ComboBoxItem).Tag as string;
                            gpib.Write($"NDIG {setNdig}");
                            GuiConfigLogTextBox.Text = $"Write: NDIG {setNdig}";
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                _ = MessageBox.Show("GPIB is not open.");
            }
        }


        private void MeasureGuiConfigButtons_Click(object sender, RoutedEventArgs e)
        {
            if (gpib.IsOpen)
            {
                try
                {
                    switch ((sender as Button).Tag as string)
                    {
                        case "ACAL":
                            if (MessageBox.Show("需要较长时间，是否继续？", "自动校准确认", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            {
                                string acal_param = (AcalComboBox.SelectedItem as ComboBoxItem).Tag as string;
                                gpib.Write("ACAL " + acal_param);
                                GuiConfigLogTextBox.Text = $"Write: ACAL {acal_param}";
                            }
                            break;
                        case "RANGE":
                            // %_resolution = (actual resolution/maximum input) × 100
                            string setRange = (RangeComboBox.SelectedItem as ComboBoxItem).Tag as string;
                            string setResolution = (ResComboBox.SelectedItem as ComboBoxItem).Tag as string;
                            string rangeCmd = $"RANGE {setRange}";
                            if (setRange != "AUTO" && setResolution != "DEFAULT")
                            {
                                decimal setRangeDecimal = Convert.ToDecimal(setRange);
                                decimal setResolutionDecimal = Convert.ToDecimal(setResolution);
                                rangeCmd += $",{setResolutionDecimal / setRangeDecimal / 10000}";
                            }
                            gpib.Write(rangeCmd);
                            GuiConfigLogTextBox.Text = $"Write: {rangeCmd}";
                            break;
                        case "NPLC":
                            string nplcCmd = $"NPLC {NplcTextBox.Text}";
                            gpib.Write(nplcCmd);
                            GuiConfigLogTextBox.Text = $"Write: {nplcCmd}";
                            break;
                        case "RANGE?":
                            bool isArange = gpib.QueryDemical("ARANGE?") != 0M;
                            gpib.WaitForDataAvailable();
                            decimal readRange = gpib.QueryDemical("RANGE?");
                            gpib.WaitForDataAvailable();
                            decimal readResolution = gpib.QueryDemical("RES?") * readRange * 10000;
                            GuiConfigLogTextBox.Text = $"Query: ARANGE? & RANGE? & RES?\nReturn: {(isArange ? "Auto Range, " + readRange.ToString() + "V" : readRange.ToString() + "V, " + readResolution.ToString() + "uV")}";
                            break;
                        case "NPLC?":
                            GuiConfigLogTextBox.Text = $"Query: NPLC?\nReturn: {gpib.QueryDemical("NPLC?")} NPLC";
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                _ = MessageBox.Show("GPIB is not open.");
            }
        }

        private void ManualReadDCVButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // %_resolution = (actual resolution/maximum input) × 100
                string setRange = (ManualReadDCVRangeComboBox.SelectedItem as ComboBoxItem).Tag as string;
                string setResolution = (ManualReadDCVResComboBox.SelectedItem as ComboBoxItem).Tag as string;
                string rangeCmd = $"DCV {setRange}";
                if (setRange != "AUTO" && setResolution != "DEFAULT")
                {
                    decimal setRangeDecimal = Convert.ToDecimal(setRange);
                    decimal setResolutionDecimal = Convert.ToDecimal(setResolution);
                    rangeCmd += $",{setResolutionDecimal / setRangeDecimal / 10000}";
                }
                ManualReadDCVCommandTextBlock.Text = rangeCmd;
                ManualReadDCVText.StringData = "Measuring...";
                _ = Task.Run(() =>
                {
                    ManualReadDCVText.StringData = gpib.QueryDemical(rangeCmd).ToString();
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
                gpib.Write("NPLC 0");
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
                gpib.Write("NPLC 1000");
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private void SearchSerialPortButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SerialPortNameComboBox.Items.Clear();
                string[] portList = SerialPort.GetPortNames();
                string[] portDescriptionList = HardwareInfoUtil.GetSerialPortFullName();
                for (int i = 0; i < portList.Length; ++i)
                {
                    ComboBoxItem comboBoxItem = new ComboBoxItem
                    {
                        Content = portList[i] + "|" + portDescriptionList.Where(str => str.Contains(portList[i])).First(),
                        Tag = portList[i]
                    };
                    _ = SerialPortNameComboBox.Items.Add(comboBoxItem);
                }
                if (portList.Length > 0)
                {
                    SerialPortNameComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private void OpenSerialPortButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedPortName = (SerialPortNameComboBox.SelectedItem as ComboBoxItem).Tag as string;
                int baudRate = Convert.ToInt32(((SerialPortBaudRateComboBox.SelectedItem as ComboBoxItem).Tag as string) == "Other"
                    ? ((SerialPortBaudRateComboBox.SelectedItem as ComboBoxItem).Content as TextBox).Text
                    : (SerialPortBaudRateComboBox.SelectedItem as ComboBoxItem).Tag as string);
                if (!serialPorts.Open(selectedPortName, baudRate))
                {
                    _ = MessageBox.Show("串口已被打开.");
                }
                SerialPortDeviceNameTextBlock.Text = serialPorts.SerialPortNames.Count() == 0
                    ? "No Device Connected."
                    : string.Join(",\n", serialPorts.SerialPortInstances.Select(serialPort => $"{serialPort.PortName}({serialPort.BaudRate}bps)").ToArray()) + "\nDevice Connected.";
                SerialPortDebugSelectComboBox.ItemsSource = serialPorts.SerialPortNames;
                SerialPortSendCmdSerialPortNameComboBox.ItemsSource = serialPorts.SerialPortNames;
                SerialPortRecvDataSerialPortNameComboBox.ItemsSource = serialPorts.SerialPortNames;

                if (SerialPortDebugSelectComboBox.Items.Count != 0 && SerialPortDebugSelectComboBox.SelectedIndex == -1)
                {
                    SerialPortDebugSelectComboBox.SelectedIndex = 0;
                }
                if (SerialPortSendCmdSerialPortNameComboBox.Items.Count != 0 && SerialPortSendCmdSerialPortNameComboBox.SelectedIndex == -1)
                {
                    SerialPortSendCmdSerialPortNameComboBox.SelectedIndex = 0;
                }
                if (SerialPortRecvDataSerialPortNameComboBox.Items.Count != 0 && SerialPortRecvDataSerialPortNameComboBox.SelectedIndex == -1)
                {
                    SerialPortRecvDataSerialPortNameComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private void CloseSerialPortButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedPortName = (SerialPortNameComboBox.SelectedItem as ComboBoxItem).Tag as string;
                if (!serialPorts.Close(selectedPortName))
                {
                    _ = MessageBox.Show("串口已被关闭.");
                }
                SerialPortDeviceNameTextBlock.Text = serialPorts.SerialPortNames.Count() == 0
                    ? "No Device Connected."
                    : string.Join(",\n", serialPorts.SerialPortInstances.Select(serialPort => $"{serialPort.PortName}({serialPort.BaudRate}bps)").ToArray()) + "\nDevice Connected.";
                SerialPortDebugSelectComboBox.ItemsSource = serialPorts.SerialPortNames;
                SerialPortSendCmdSerialPortNameComboBox.ItemsSource = serialPorts.SerialPortNames;
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
                // CollectionView不支持从调度程序以外的线程对其SourceCollection进行的更改
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Application.Current.Dispatcher));
                SynchronizationContext.Current.Post(p1 =>
                {
                    MultimeterDataStorage.Add(ManualReadDCVText.Clone());
                }, null);
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
                    //serialPortRecvDataStorage.AddRange(objArray);
                    foreach (object obj in objArray)
                    {
                        SerialPortRecvDataStorage.Add(new StringDataBinding { StringData = obj.ToString() });
                    }
                }
                else
                {
                    //serialPortRecvDataStorage.Add(RecvDataPraseTemp);
                    SerialPortRecvDataStorage.Add(new StringDataBinding { StringData = RecvDataPraseTemp.ToString() });
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private void DataStorageSelectListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (DataStorageDataGrid != null)
                {
                    switch ((DataStorageSelectListBox.SelectedItem as ListBoxItem).Tag)
                    {
                        case "Multimeter":
                            DataStorageDataGrid.ItemsSource = MultimeterDataStorage;
                            break;
                        case "SerialPort":
                            DataStorageDataGrid.ItemsSource = SerialPortRecvDataStorage;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private void DataStorageExportListDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dataStorageTag = (DataStorageSelectListBox.SelectedItem as ListBoxItem).Tag as string;
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "存储数据",
                    FileName = $"{dataStorageTag}DataStorage_{DateTime.Now.ToString().Replace('/', '-').Replace(':', '-').Replace(' ', '-')}.txt",
                    DefaultExt = ".txt",
                    Filter = "CSV File|*.txt"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    switch (dataStorageTag)
                    {
                        case "Multimeter":
                            File.WriteAllLines(saveFileDialog.FileName, MultimeterDataStorage.Select(strcls => strcls.StringData));
                            break;
                        case "SerialPort":
                            File.WriteAllLines(saveFileDialog.FileName, SerialPortRecvDataStorage.Select(strcls => strcls.StringData));
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private void DataStorageClearListDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("清理本次通信数据，是否继续？", "清理数据确认", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    switch ((DataStorageSelectListBox.SelectedItem as ListBoxItem).Tag)
                    {
                        case "Multimeter":
                            MultimeterDataStorage.Clear();
                            break;
                        case "SerialPort":
                            SerialPortRecvDataStorage.Clear();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }
    }
}
