using CarrotProtocolLib.Interface;
using CarrotProtocolLib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Device
{
    public partial class EmptyDevice : ObservableObject, IDevice
    {
        [ObservableProperty]
        public int receivedByteCount;
        [ObservableProperty]
        public int sentByteCount;

        [ObservableProperty]
        public bool isOpen;

        public int RxByteToRead { get; set; }

        public static EmptyDevice EmptyDeviceInstance { get; } = new EmptyDevice();
        public SerialPort Driver { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public RingBuffer RxBuffer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event IDevice.DevicePropertyChangedHandler DevicePropertyChanged;

        public EmptyDevice()
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

        public static DeviceInfo[] GetDevicesInfo()
        {
            return new DeviceInfo[] { };
        }
    }
}
