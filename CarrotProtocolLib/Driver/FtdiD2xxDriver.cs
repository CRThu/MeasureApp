using CarrotProtocolLib.Device;
using CommunityToolkit.Mvvm.ComponentModel;
using FTD2XX_NET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static FTD2XX_NET.FTDI;

namespace CarrotProtocolLib.Driver
{
    /// <summary>
    /// FTDI FT2232H SYNC FIFO Driver
    /// </summary>
    public partial class FtdiD2xxDriver : ObservableObject, IDriver
    {
        /// <summary>
        /// FTDI驱动包装类
        /// </summary>
        private FTDI FtdiDevice { get; set; }

        /// <summary>
        /// 返回状态(内部使用)
        /// </summary>
        private FTDI.FT_STATUS FtStatus { get; set; }

        /// <summary>
        /// 读取中转缓冲器(内部使用)
        /// </summary>
        private byte[] ReadBuffer { get; set; }

        /// <summary>
        /// 设备是否打开
        /// </summary>
        [ObservableProperty]
        private bool isOpen;

        /// <summary>
        /// FIFO待读取字节
        /// </summary>
        public int BytesToRead => GetBytesToRead();

        /// <summary>
        /// 接收字节数
        /// </summary>
        [ObservableProperty]
        private int receivedByteCount;

        /// <summary>
        /// 发送字节数
        /// </summary>
        [ObservableProperty]
        private int sentByteCount;

        public event IDriver.ErrorReceivedHandler? ErrorReceived;

        public FtdiD2xxDriver()
        {
            FtdiDevice = new FTDI();
            FtStatus = FTDI.FT_STATUS.FT_OK;
            ReadBuffer = new byte[1024 * 1024];
            IsOpen = false;
        }

        /// <summary>
        /// 配置驱动
        /// </summary>
        /// <param name="timeout"></param>
        public void SetDriver(string serialNumber, int timeout = 250)
        {
            // Open Device before Config
            // Open first device in our list by serial number
            FtStatus = FtdiDevice.OpenBySerialNumber(serialNumber);
            if (FtStatus != FTDI.FT_STATUS.FT_OK)
            {
                Debug.WriteLine($"Failed to open device (error {FtStatus})");
                return;
            }
            IsOpen = true;

            if (IsOpen)
            {
                // Set Timeout
                FtStatus = FtdiDevice.SetTimeouts((uint)timeout, (uint)timeout);
                if (FtStatus != FTDI.FT_STATUS.FT_OK)
                {
                    // FT_SetTimeouts failed
                    Debug.WriteLine($"Failed to Set Timeouts (error {FtStatus})");
                }

                // Set BitMode
                // SYNC FIFO MODE NEED BOTH WRITE EEPROM AND SETBITMODE
                byte mask, mode;
                mask = 0xff;
                //   BitMode:
                //     For FT232H devices, valid values are FT_BIT_MODE_RESET, FT_BIT_MODE_ASYNC_BITBANG, FT_BIT_MODE_MPSSE, FT_BIT_MODE_SYNC_BITBANG, FT_BIT_MODE_CBUS_BITBANG, FT_BIT_MODE_MCU_HOST, FT_BIT_MODE_FAST_SERIAL, FT_BIT_MODE_SYNC_FIFO.
                //     For FT2232H devices, valid values are FT_BIT_MODE_RESET, FT_BIT_MODE_ASYNC_BITBANG, FT_BIT_MODE_MPSSE, FT_BIT_MODE_SYNC_BITBANG, FT_BIT_MODE_MCU_HOST, FT_BIT_MODE_FAST_SERIAL, FT_BIT_MODE_SYNC_FIFO.
                //     For FT4232H devices, valid values are FT_BIT_MODE_RESET, FT_BIT_MODE_ASYNC_BITBANG, FT_BIT_MODE_MPSSE, FT_BIT_MODE_SYNC_BITBANG.
                //     For FT232R devices, valid values are FT_BIT_MODE_RESET, FT_BIT_MODE_ASYNC_BITBANG, FT_BIT_MODE_SYNC_BITBANG, FT_BIT_MODE_CBUS_BITBANG.
                //     For FT245R devices, valid values are FT_BIT_MODE_RESET, FT_BIT_MODE_ASYNC_BITBANG, FT_BIT_MODE_SYNC_BITBANG.
                //     For FT2232 devices, valid values are FT_BIT_MODE_RESET, FT_BIT_MODE_ASYNC_BITBANG, FT_BIT_MODE_MPSSE, FT_BIT_MODE_SYNC_BITBANG, FT_BIT_MODE_MCU_HOST, FT_BIT_MODE_FAST_SERIAL.
                //     For FT232B and FT245B devices, valid values are FT_BIT_MODE_RESET, FT_BIT_MODE_ASYNC_BITBANG.
                mode = FT_BIT_MODES.FT_BIT_MODE_SYNC_FIFO;
                FtStatus = FtdiDevice.SetBitMode(mask, mode);
                if (FtStatus != FTDI.FT_STATUS.FT_OK)
                {
                    // FT_SetBitMode FAILED!
                    Debug.WriteLine($"Failed to Set Bit Mode (error {FtStatus})");
                }

            }
            else
            {
                Debug.WriteLine($"device is closed.");
            }
        }

        public void Open()
        {
            // 实际打开步骤已在SetDriver函数内运行
            if (!IsOpen)
            {
                Debug.WriteLine($"device is not open.");
            }
        }

        public void Close()
        {
            if (IsOpen)
            {
                // Close our device
                FtStatus = FtdiDevice.Close();
                if (FtStatus != FTDI.FT_STATUS.FT_OK)
                {
                    Debug.WriteLine($"Failed to close device (error {FtStatus})");
                    return;
                }
                IsOpen = false;
            }
            else
            {
                Debug.WriteLine($"device is closed.");
            }
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (IsOpen)
            {
                byte[] bufferWithZeroOffset = buffer;
                uint numBytesWritten = 0;
                if (offset != 0)
                {
                    bufferWithZeroOffset = buffer.Skip(offset).ToArray();
                }

                FtStatus = FtdiDevice.Write(bufferWithZeroOffset, count, ref numBytesWritten);
                if (FtStatus == FTDI.FT_STATUS.FT_OK)
                {
                    // FT_Write OK

                    // Waiting for transfer done
                    while (count != numBytesWritten)
                    {
                        Debug.WriteLine($"Waiting for write device done ({numBytesWritten}/{count})");
                    }

                    SentByteCount += count;
                }
                else
                {
                    // FT_Write Failed
                    Debug.WriteLine($"Failed to write device (error {FtStatus})");
                }
            }
            else
            {
                Debug.WriteLine($"device is closed.");
            }
        }

        public int Read(byte[] buffer, int offset, int bytesExpected)
        {
            uint currentBytesRead = 0;
            uint currentBytesExpected = 0;
            int totalBytesRead = 0;

            while (bytesExpected > 0)
            {
                // 一次读取不超过读取缓冲区的长度字节流
                currentBytesExpected = (uint)bytesExpected;
                if (currentBytesExpected > ReadBuffer.Length)
                {
                    currentBytesExpected = (uint)ReadBuffer.Length;
                }

                // Read
                FtStatus = FtdiDevice.Read(ReadBuffer, currentBytesExpected, ref currentBytesRead);
                if (FtStatus != FTDI.FT_STATUS.FT_OK)
                {
                    // FT_Read Failed
                    Debug.WriteLine($"Waiting for read device (error {FtStatus})");
                }

                // Copy to Buffer
                Array.Copy(ReadBuffer, 0, buffer, offset, currentBytesRead);

                offset += (int)currentBytesRead;
                totalBytesRead += (int)currentBytesRead;
                bytesExpected -= (int)currentBytesRead;
            }

            ReceivedByteCount += totalBytesRead;
            return totalBytesRead;
        }

        public static DeviceInfo[] GetDevicesInfo()
        {
            UInt32 ftdiDeviceCount = 0;

            // Create new instance of the FTDI device class
            FTDI FtdiDevice = new FTDI();
            FTDI.FT_STATUS FtStatus = FT_STATUS.FT_OK;
            Debug.WriteLine(FtdiDevice.GetType().Assembly.Location);

            // Determine the number of FTDI devices connected to the machine
            FtStatus = FtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
            // Check status
            if (FtStatus == FTDI.FT_STATUS.FT_OK)
            {
                Debug.WriteLine("Number of FTDI devices: " + ftdiDeviceCount.ToString());
            }
            else
            {
                // Wait for a key press
                Debug.WriteLine("Failed to get number of devices (error " + FtStatus.ToString() + ")");
                return Array.Empty<DeviceInfo>();
            }

            // If no devices available, return
            if (ftdiDeviceCount == 0)
            {
                Debug.WriteLine("Failed to get number of devices (error " + FtStatus.ToString() + ")");
                return Array.Empty<DeviceInfo>();
            }

            // Allocate storage for device info list
            FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];

            // Populate our device list
            FtStatus = FtdiDevice.GetDeviceList(ftdiDeviceList);

            if (FtStatus == FTDI.FT_STATUS.FT_OK)
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
                return ftdiDeviceList.Select(dev => new DeviceInfo(dev.Type.ToString(), dev.SerialNumber, dev.Description))
                    .ToArray();
            }
            else
            {
                Debug.WriteLine("Failed to get device list (error " + FtStatus.ToString() + ")");
                return Array.Empty<DeviceInfo>();
            }

        }


        private int GetBytesToRead()
        {
            uint rxQuene = 0;
            FtdiDevice.GetRxBytesAvailable(ref rxQuene);
            if (FtStatus == FTDI.FT_STATUS.FT_OK)
            {
                return (int)rxQuene;
            }
            else
            {
                Debug.WriteLine("Failed to get Queue Status (error " + FtStatus.ToString() + ")");
                return 0;
            }
        }

    }
}
