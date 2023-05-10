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
using CarrotProtocolLib.Util;
using CarrotProtocolLib.Impl;
using CarrotProtocolLib.Driver;

namespace CarrotProtocolCommDemo
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public SerialPortDevice SerialPortDevice { get; set; }
        public ProtocolLogger Logger { get; set; }
        public DeviceProtocol Protocol { get; set; }


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

        [ObservableProperty]
        private bool isOpen = false;

        public MainWindowViewModel()
        {
            SerialPortNames = SerialPort.GetPortNames();
            SelectedSerialPortName = SerialPortNames.FirstOrDefault() ?? "";
            CarrotProtocols = new int[] { 0x30, 0x31, 0x32, 0x33 };
            SelectedCarrotProtocol = CarrotProtocols.FirstOrDefault();
            CarrotProtocolStreamIds = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            SelectedCarrotProtocolStreamId = CarrotProtocolStreamIds.FirstOrDefault();

            SerialPortDevice = new();
            //InputCode = GenHexPkt();
        }

        [RelayCommand]
        private void Open()
        {
            try
            {
                //MessageBox.Show("Open");
                if (!SerialPortDevice.IsOpen)
                {
                    SerialPortDevice.SetDevice(SelectedSerialPortName, 115200);
                    Logger = new();
                    Protocol = new(SerialPortDevice, Logger);
                    Protocol.ReceiveError += Protocol_ReceiveError;
                    Protocol.Start();
                    Logger.LoggerUpdate += Logger_LoggerUpdate;
                }
                else
                {
                    Protocol.Stop();
                }
                IsOpen = SerialPortDevice.IsOpen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Protocol_ReceiveError(Exception ex)
        {
            MessageBox.Show(ex.ToString());
            Protocol.Stop();
        }

        [RelayCommand]
        private void PacketParamsChanged()
        {
            try
            {
                CarrotDataProtocol carrotDataProtocol = new(SelectedCarrotProtocol, SelectedCarrotProtocolStreamId, PayloadString);
                byte[] bytes = carrotDataProtocol.ToBytes();
                InputCode = BytesEx.BytesToHexString(bytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
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
            try
            {
                //MessageBox.Show("Send");
                Protocol.Send(BytesEx.HexStringToBytes(InputCode));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
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
