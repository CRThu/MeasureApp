using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
using NationalInstruments.VisaNS;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        GPIB gpib = new GPIB();
        public ObservableCollection<string> gpibDeviceNames = new ObservableCollection<string>();
        public DispatcherTimer gpibStbTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            SearchGPIBDevicesButton_Click(null, null);

            DeviceComboBox.ItemsSource = gpibDeviceNames;

            gpibStbTimer.Tick += GpibStbTimer_Tick;
            gpibStbTimer.Interval = TimeSpan.FromMilliseconds(Properties.Settings.Default.STBInterval);
        }

        private void GpibStbTimer_Tick(object sender, EventArgs e)
        {
            StbStatusBar.Text = $"IsReadyForInstructions={gpib.IsReadyForInstructions}, IsDataAvailable={gpib.IsDataAvailable}";
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
                gpibStbTimer.Stop();
                gpib.Dispose();
                gpib.Open(DeviceComboBox.SelectedItem as string);
                gpibStbTimer.Start();
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
                    ReadCmdTextBox.Text = gpib.Query(WriteCmdTextBox.Text);
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

        private void Window_Closed(object sender, EventArgs e)
        {
            gpib.Dispose();
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
    }
}
