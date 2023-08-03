using CarrotProtocolLib.Interface;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using FTD2XX_NET;
using System.IO.Ports;
using CarrotProtocolLib.Util;

namespace CarrotProtocolLib.Device
{
    public partial class FtdiD2xxDevice : ObservableObject, IDevice
    {
        private FTDI D2xx { get; set; }

        [ObservableProperty]
        public int receivedByteCount;
        [ObservableProperty]
        public int sentByteCount;

        [ObservableProperty]
        public bool isOpen;

        public int RxByteToRead { get; set; }

        public SerialPort Driver { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public RingBuffer RxBuffer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public FtdiD2xxDevice()
        {
            ReceivedByteCount = 0;
            SentByteCount = 0;
            RxByteToRead = 0;
            IsOpen = false;
        }

        public void Open()
        {
            IsOpen = true;
        }

        public void Close()
        {
            IsOpen = false;
        }

        public void Write(byte[] bytes)
        {
            SentByteCount += bytes.Length;
        }

        public void Read(byte[] responseBytes, int offset, int bytesExpected)
        {
            throw new NotImplementedException();
        }


        public void SetDevice(string serialNumber)
        {
        }
    }
}
