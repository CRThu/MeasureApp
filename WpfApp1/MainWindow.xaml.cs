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
        public ObservableCollection<string> gpibDeviceNames = new ObservableCollection<string>(GPIB.SearchDevices());
        public MainWindow()
        {
            InitializeComponent();
            deviceComboBox.ItemsSource = gpibDeviceNames;

            if (gpibDeviceNames.Count != 0 && deviceComboBox.SelectedIndex == -1)
            {
                deviceComboBox.SelectedIndex = 0;
            }
        }

        private void SearchGPIBDevicesButton_Click(object sender, RoutedEventArgs e)
        {
            gpibDeviceNames.Clear();
            foreach (string resource in GPIB.SearchDevices())
            {
                gpibDeviceNames.Add(resource);
            }

            if (gpibDeviceNames.Count != 0 && deviceComboBox.SelectedIndex == -1)
            {
                deviceComboBox.SelectedIndex = 0;
            }
        }
        private void OpenGPIBDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            gpib.Dispose();
            gpib.Open((string)deviceComboBox.SelectedItem);
            gpib.Write("END");
            string deviceName = gpib.Query("ID?");
            deviceNameLabel.Content = deviceName;
        }

        private void WriteCmdButton_Click(object sender, RoutedEventArgs e)
        {
            if (gpib.isOpen)
            {
                gpib.Write(writeCmdTextBox.Text);
            }
            else
            {
                _ = MessageBox.Show("GPIB is not open.");
            }
        }

        private void QueryCmdButton_Click(object sender, RoutedEventArgs e)
        {
            if (gpib.isOpen)
            {
                readCmdTextBox.Text = gpib.Query(writeCmdTextBox.Text);
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

    }
}
