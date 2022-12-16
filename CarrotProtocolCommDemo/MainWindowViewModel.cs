using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.IO.Ports;
using System.Diagnostics;

namespace CarrotProtocolCommDemo
{
    public partial class MainWindowViewModel : ObservableObject
    {
        SerialPortDevice spd;
        ProtocolLogger logger;
        DeviceProtocol protocol;


        [ObservableProperty]
        private string[] serialPortNames;

        [ObservableProperty]
        private string selectedSerialPortName;

        [ObservableProperty]
        private int[] carrotProtocols;

        [ObservableProperty]
        private int selectedCarrotProtocol;

        [ObservableProperty]
        private int[] carrotProtocolStreamIds;

        [ObservableProperty]
        private int selectedCarrotProtocolStreamId;

        [ObservableProperty]
        private string payloadString = "";


        [ObservableProperty]
        private string inputCode = "";

        [ObservableProperty]
        private string stdOut = "";

        public MainWindowViewModel()
        {
            SerialPortNames = SerialPort.GetPortNames();
            SelectedSerialPortName = SerialPortNames.FirstOrDefault() ?? "";
            CarrotProtocols = new int[] { 0x30, 0x31, 0x32, 0x33 };
            SelectedCarrotProtocol = CarrotProtocols.FirstOrDefault();
            CarrotProtocolStreamIds = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            SelectedCarrotProtocolStreamId = CarrotProtocolStreamIds.FirstOrDefault();

            //InputCode = GenHexPkt();
        }

        [RelayCommand]
        private void Open()
        {
            //MessageBox.Show("Open");
            spd = new(SelectedSerialPortName, 921600);
            spd.Open();
            logger = new();
            protocol = new(spd, logger);
            logger.LoggerUpdate += Logger_LoggerUpdate;
        }

        [RelayCommand]
        private void PacketParamsChanged()
        {
            CarrotDataProtocol carrotDataProtocol = new(SelectedCarrotProtocol, SelectedCarrotProtocolStreamId, PayloadString);
            byte[] bytes = carrotDataProtocol.ToBytes();
            InputCode = BytesEx.BytesToHexString(bytes);
        }

        private void Logger_LoggerUpdate(ProtocolLog log, LoggerUpdateEvent e)
        {
            lock (StdOut)
            {
                StdOut += log.ToString() + Environment.NewLine;
            }
        }

        [RelayCommand]
        private void Send()
        {
            //MessageBox.Show("Send");
            protocol.Send(BytesEx.HexStringToBytes(InputCode));
        }

        public static string GenHexPkt()
        {
            string h = "3C 30 22 11 08 05 00 11 22 33 44 55 66 EE EE 3E";
            //string h = "44 03 FF FF 08 05 00 ";

            //for (int i = 0; i < 16-10; i++)
            //{
            //    h += "AA ";
            //}

            //h += "FF FF 55";
            return h;
        }
    }
}
