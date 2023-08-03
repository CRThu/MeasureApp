using CarrotProtocolLib.Device;
using FTD2XX_NET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Driver
{
    public static class FtdiD2xxHelper
    {
        public static FTDI.FT_DEVICE_INFO_NODE[] GetDevicesInfo()
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
                Debug.WriteLine("Number of FTDI devices: " + ftdiDeviceCount.ToString());
                Debug.WriteLine("");
            }
            else
            {
                // Wait for a key press
                Debug.WriteLine("Failed to get number of devices (error " + ftStatus.ToString() + ")");
                return Array.Empty<FTDI.FT_DEVICE_INFO_NODE>();
            }

            // If no devices available, return
            if (ftdiDeviceCount == 0)
            {
                // Wait for a key press
                Debug.WriteLine("Failed to get number of devices (error " + ftStatus.ToString() + ")");
                return Array.Empty<FTDI.FT_DEVICE_INFO_NODE>();
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
            new DeviceInfo("FTD2XX_NET.FTDI_D2XX", dev.SerialNumber, dev.Description))
                .ToArray();
        }
    }
}
