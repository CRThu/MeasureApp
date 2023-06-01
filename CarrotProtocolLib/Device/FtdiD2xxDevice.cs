using CarrotProtocolLib.Interface;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using FTD2XX_NET;

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

        public static EmptyDevice EmptyDeviceInstance { get; } = new EmptyDevice();

        public event IDevice.OnInternalPropertyChangedHandler InternalPropertyChanged;

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

        public static DeviceInfo[] GetDevicesInfo()
        {
            UInt32 ftdiDeviceCount = 0;
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;

            // Create new instance of the FTDI device class
            FTDI myFtdiDevice = new FTDI();
            Debug.WriteLine(myFtdiDevice.GetType().Assembly.Location);

            // Determine the number of FTDI devices connected to the machine
            ftStatus = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
            // Check status
            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                Console.WriteLine("Number of FTDI devices: " + ftdiDeviceCount.ToString());
                Console.WriteLine("");
            }
            else
            {
                // Wait for a key press
                Console.WriteLine("Failed to get number of devices (error " + ftStatus.ToString() + ")");
                Console.ReadKey();
                return new DeviceInfo[] { };
            }

            // If no devices available, return
            if (ftdiDeviceCount == 0)
            {
                // Wait for a key press
                Console.WriteLine("Failed to get number of devices (error " + ftStatus.ToString() + ")");
                Console.ReadKey();
                return new DeviceInfo[] { };
            }

            // Allocate storage for device info list
            FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];

            // Populate our device list
            ftStatus = myFtdiDevice.GetDeviceList(ftdiDeviceList);

            if (ftStatus == FTDI.FT_STATUS.FT_OK)
            {
                for (UInt32 i = 0; i < ftdiDeviceCount; i++)
                {
                    Debug.WriteLine("Device Index: " + i.ToString());
                    Debug.WriteLine("Flags: " + String.Format("{0:x}", ftdiDeviceList[i].Flags));
                    Debug.WriteLine("Type: " + ftdiDeviceList[i].Type.ToString());
                    Debug.WriteLine("ID: " + String.Format("{0:x}", ftdiDeviceList[i].ID));
                    Debug.WriteLine("Location ID: " + String.Format("{0:x}", ftdiDeviceList[i].LocId));
                    Debug.WriteLine("Serial Number: " + ftdiDeviceList[i].SerialNumber.ToString());
                    Debug.WriteLine("Description: " + ftdiDeviceList[i].Description.ToString());
                    Debug.WriteLine("");
                }
            }

            return ftdiDeviceList.Select(dev =>
            new DeviceInfo(InterfaceType.FTDI_D2XX, dev.SerialNumber, dev.Description))
                .ToArray();
        }

        public void SetDevice(string serialNumber)
        {
        }
    }
}
