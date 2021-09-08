using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
        public MainWindow()
        {
            InitializeComponent();

            SearchGPIBDevicesButton_Click(null, null);

            deviceComboBox.ItemsSource = gpibDeviceNames;
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
                _ = MessageBox.Show(ex.ToString());
            }

            if (gpibDeviceNames.Count != 0 && deviceComboBox.SelectedIndex == -1)
            {
                deviceComboBox.SelectedIndex = 0;
            }
        }
        private void OpenGPIBDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                gpib.Dispose();
                gpib.Open((string)deviceComboBox.SelectedItem);
                gpib.messageBasedSession.Timeout = Properties.Settings.Default.GPIBTimeout;
                gpib.Write("END");
                string deviceName = gpib.Query("ID?");
                deviceNameLabel.Content = deviceName;
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private void QueryCmdButton_Click(object sender, RoutedEventArgs e)
        {
            if (gpib.isOpen)
            {
                try
                {
                    readCmdTextBox.Text = gpib.Query(writeCmdTextBox.Text);
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
            if (gpib.isOpen)
            {
                try
                {
                    gpib.Write(writeCmdTextBox.Text);
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
            if (gpib.isOpen)
            {
                try
                {
                    readCmdTextBox.Text = gpib.ReadString();
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
            if (gpib.isOpen)
            {
                try
                {
                    switch ((string)((Button)sender).Tag)
                    {
                        case "RESET":
                            gpib.Write("RESET");
                            gpib.Write("END");
                            break;
                        case "ID":
                            GuiConfigLogTextBox.Text = gpib.Query("ID?");
                            break;
                        case "ERR":
                            GuiConfigLogTextBox.Text = gpib.Query("ERRSTR?");
                            break;
                        case "STB":
                            GuiConfigLogTextBox.Text = gpib.Query("STB?");
                            break;
                        case "TEMP":
                            GuiConfigLogTextBox.Text = gpib.Query("TEMP?");
                            break;
                        case "LFREQ":
                            GuiConfigLogTextBox.Text = gpib.Query("LFREQ?");
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
