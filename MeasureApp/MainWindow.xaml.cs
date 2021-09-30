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
using MeasureApp.ViewModel;
using System.Reflection;
using System.Diagnostics;

namespace MeasureApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowDataContext mainWindowDataContext = new MainWindowDataContext();

        GPIB3458AMeasure measure3458A = new GPIB3458AMeasure();

        DataStorage dataStorage = new DataStorage();
        string Key3458AString = "3458A Data Storage";

        public SerialPorts serialPorts = new SerialPorts();

        public ObservableCollection<string> gpibDeviceNames = new ObservableCollection<string>();

        public bool IsSyncDCVDisplay = false;
        private ManualResetEvent resetEvent = new ManualResetEvent(false);
        public StringDataClass SyncDCVDisplayText = new StringDataClass() { StringData = "<null>" };
        public BoolDataBinding SyncDCVIsAutoStorage = new BoolDataBinding() { BoolData = false };
        public StringDataClass ManualReadDCVText = new StringDataClass() { StringData = "<null>" };
        public StringDataClass SerialPortSendCmdString = new StringDataClass() { StringData = "<null>::<null>;" };

        public SerialPortRecvDataType serialPortRecvDataType = new SerialPortRecvDataType();
        private dynamic RecvDataPraseTemp;
        //public ObservableCollection<StringDataClass> MultimeterDataStorage = new ObservableCollection<StringDataClass>();
        public ObservableCollection<StringDataClass> SerialPortRecvDataStorage = new ObservableCollection<StringDataClass>();

        public MainWindow()
        {
            InitializeComponent();

            //DataContext = new MainWindowDataContext();
            DataContext = mainWindowDataContext;

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
            measure3458A.Dispose();
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
            catch (Exception)
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
                measure3458A.Dispose();
                string deviceName = measure3458A.Open(DeviceComboBox.SelectedItem as string);
                measure3458A.Timeout = Properties.Settings.Default.GPIBTimeout;
                DeviceNameLabel.Text = deviceName;
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private void QueryCmdButton_Click(object sender, RoutedEventArgs e)
        {
            if (measure3458A.IsOpen)
            {
                try
                {
                    ReadCmdTextBox.Text = measure3458A.QueryCommand(WriteCmdTextBox.Text).ToString();
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
            if (measure3458A.IsOpen)
            {
                try
                {
                    measure3458A.WriteCommand(WriteCmdTextBox.Text);
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
            if (measure3458A.IsOpen)
            {
                try
                {
                    ReadCmdTextBox.Text = measure3458A.ReadString();
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
                                    // CollectionView不支持从调度程序以外的线程对其SourceCollection进行的更改
                                    SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Application.Current.Dispatcher));
                                    SynchronizationContext.Current.Post(p1 =>
                                    {
                                        dataStorage.AddData(Key3458AString, DCVDisplay);
                                        // MultimeterDataStorage.Add(new StringDataClass() { StringData = DCVDisplay.ToString() });
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
            if (measure3458A.IsOpen)
            {
                try
                {
                    switch ((sender as Button).Tag as string)
                    {
                        case "RESET":
                            measure3458A.Reset();
                            GuiConfigLogTextBox.Text = $"Write: RESET & END";
                            break;
                        case "ID":
                            GuiConfigLogTextBox.Text = $"Query: ID?\nReturn: {measure3458A.GetID()}";
                            break;
                        case "ERR":
                            GuiConfigLogTextBox.Text = $"Query: ERRSTR?\nReturn: {measure3458A.GetErrorString()}";
                            break;
                        case "STB":
                            GuiConfigLogTextBox.Text = $"Query: STB?\nReturn: {measure3458A.ReadStatusByte()}";
                            break;
                        case "TEMP":
                            GuiConfigLogTextBox.Text = $"Query: TEMP?\nReturn: {measure3458A.GetTemp()}";
                            break;
                        case "LINE":
                            GuiConfigLogTextBox.Text = $"Query: LINE?\nReturn: {measure3458A.GetLineFreq()} Hz";
                            break;
                        case "NDIG":
                            string setNdig = (NdigComboBox.SelectedItem as ComboBoxItem).Tag as string;
                            measure3458A.WriteCommand($"NDIG {setNdig}");
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
            if (measure3458A.IsOpen)
            {
                try
                {
                    switch ((sender as Button).Tag as string)
                    {
                        case "ACAL":
                            if (MessageBox.Show("需要较长时间，是否继续？", "自动校准确认", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            {
                                string acal_param = (AcalComboBox.SelectedItem as ComboBoxItem).Tag as string;
                                measure3458A.WriteCommand("ACAL " + acal_param);
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
                            measure3458A.WriteCommand(rangeCmd);
                            GuiConfigLogTextBox.Text = $"Write: {rangeCmd}";
                            break;
                        case "NPLC":
                            string nplcCmd = $"NPLC {NplcTextBox.Text}";
                            measure3458A.WriteCommand(nplcCmd);
                            GuiConfigLogTextBox.Text = $"Write: {nplcCmd}";
                            break;
                        case "RANGE?":
                            bool isArange = measure3458A.QueryDecimal("ARANGE?") != 0M;
                            measure3458A.WaitForDataAvailable();
                            decimal readRange = measure3458A.QueryDecimal("RANGE?");
                            measure3458A.WaitForDataAvailable();
                            decimal readResolution = measure3458A.QueryDecimal("RES?") * readRange * 10000;
                            GuiConfigLogTextBox.Text = $"Query: ARANGE? & RANGE? & RES?\nReturn: {(isArange ? "Auto Range, " + readRange.ToString() + "V" : readRange.ToString() + "V, " + readResolution.ToString() + "uV")}";
                            break;
                        case "NPLC?":
                            GuiConfigLogTextBox.Text = $"Query: NPLC?\nReturn: {measure3458A.GetNPLC()} NPLC";
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
                    dataStorage.AddData(Key3458AString, Convert.ToDecimal(ManualReadDCVText));
                    // MultimeterDataStorage.Add(ManualReadDCVText.Clone());
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
                    foreach (object obj in objArray)
                    {
                        SerialPortRecvDataStorage.Add(new StringDataClass { StringData = obj.ToString() });
                    }
                }
                else
                {
                    SerialPortRecvDataStorage.Add(new StringDataClass { StringData = RecvDataPraseTemp.ToString() });
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
                            //BUG
                            DataStorageDataGrid.ItemsSource = dataStorage.DataStorageDictionary[Key3458AString];
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
                            File.WriteAllLines(saveFileDialog.FileName, dataStorage.DataStorageDictionary[Key3458AString].Select<dynamic, string>(i => i?.ToString()).ToList());
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
                            dataStorage.DataStorageDictionary[Key3458AString].Clear();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mainWindowDataContext.AutoTextBox = "dcv = MeasureDCV(10, 0.0001); nplc = GetNPLC();";

                CodeParse codeParse = new(mainWindowDataContext.AutoTextBox);
                codeParse.ExecuteAllCodes(measure3458A);
                foreach (KeyValuePair<string, dynamic> keyValuePair in codeParse.ProcessVariables)
                {
                    Debug.WriteLine($"'{keyValuePair.Key}': {keyValuePair.Value.ToString()}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
