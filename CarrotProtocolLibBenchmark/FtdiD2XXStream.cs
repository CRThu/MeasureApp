using FTD2XX_NET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CarrotProtocolLibBenchmark
{
    internal class FtdiD2xxStream : Stream
    {
        /// <summary>
        /// FTDI设备实例
        /// </summary>
        private FTDI? ftHandle;

        /// <summary>
        /// 返回状态(内部使用)
        /// </summary>
        private FTDI.FT_STATUS ftStatus;

        /// <summary>
        /// FTDI设备实例是否打开
        /// </summary>
        public bool IsOpen => (ftHandle != null) && (ftHandle.IsOpen);

        /// <summary>
        /// 是否支持读取
        /// </summary>
        public override bool CanRead => IsOpen;

        /// <summary>
        /// 是否支持查找
        /// </summary>
        public override bool CanSeek => false;

        /// <summary>
        /// 是否支持写入
        /// </summary>
        public override bool CanWrite => IsOpen;

        /// <summary>
        /// FtdiD2XXStream 不支持查找, Length 抛出 <see cref="NotSupportedException"/>
        /// </summary>
        public override long Length => throw new NotSupportedException();

        public int ByteToRead => GetBytesToRead();
        public int ByteToWrite => GetBytesToWrite();

        private byte[] writeBuffer;
        private readonly byte[] readBuffer = new byte[1024 * 1024];

        /// <summary>
        /// FtdiD2XXStream 不支持查找, Position 抛出 <see cref="NotSupportedException"/>
        /// </summary>
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public FtdiD2xxStream(string serialNumber, int timeout = 250) : base()
        {
            ftHandle = new FTDI();
            ftStatus = FTDI.FT_STATUS.FT_OK;
            // Open Device before Config
            // Open first device in our list by serial number
            ftStatus = ftHandle.OpenBySerialNumber(serialNumber);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                throw new IOException($"Failed to open device (error {ftStatus})");
            }

            if (IsOpen)
            {
                // Set Timeout
                ftStatus = ftHandle.SetTimeouts((uint)timeout, (uint)timeout);
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    // FT_SetTimeouts failed
                    throw new IOException($"Failed to Set Timeouts (error {ftStatus})");
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
                mode = FTDI.FT_BIT_MODES.FT_BIT_MODE_SYNC_FIFO;
                ftStatus = ftHandle.SetBitMode(mask, mode);
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    // FT_SetBitMode FAILED!
                    throw new IOException($"Failed to Set Bit Mode (error {ftStatus})");
                }

            }
            else
            {
                throw new IOException($"device is closed.");
            }
        }

        ~FtdiD2xxStream()
        {
            if (IsOpen)
            {
                ftStatus = ftHandle!.Close();
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    throw new IOException($"Failed to close device (error {ftStatus})");
                }
            }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            uint currentBytesRead = 0;
            uint currentBytesExpected = 0;
            int totalBytesRead = 0;

            while (count > 0)
            {
                // 一次读取不超过读取缓冲区的长度字节流
                currentBytesExpected = (uint)count;
                if (currentBytesExpected > readBuffer.Length)
                {
                    currentBytesExpected = (uint)readBuffer.Length;
                }

                // Read
                ftStatus = ftHandle!.Read(readBuffer, currentBytesExpected, ref currentBytesRead);
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    // FT_Read Failed
                    throw new IOException($"Waiting for read device (error {ftStatus})");
                }

                // Copy to Buffer
                Array.Copy(readBuffer, 0, buffer, offset, currentBytesRead);

                offset += (int)currentBytesRead;
                totalBytesRead += (int)currentBytesRead;
                count -= (int)currentBytesRead;
            }
            return totalBytesRead;
        }

        /// <summary>
        /// FtdiD2XXStream 不支持查找, Seek 抛出 <see cref="NotSupportedException"/>
        /// </summary>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// FtdiD2XXStream 不支持查找, SetLength 抛出 <see cref="NotSupportedException"/>
        /// </summary>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (IsOpen)
            {
                writeBuffer = buffer;
                uint numBytesWritten = 0;
                if (offset != 0)
                {
                    writeBuffer = buffer.AsSpan().Slice(offset, count).ToArray();
                    //writeBuffer = buffer.Skip(offset).ToArray();
                }

                ftStatus = ftHandle!.Write(writeBuffer, count, ref numBytesWritten);
                if (ftStatus == FTDI.FT_STATUS.FT_OK)
                {
                    // FT_Write OK

                    // Waiting for transfer done
                    while (count != numBytesWritten)
                    {
                        Debug.WriteLine($"Waiting for write device done ({numBytesWritten}/{count})");
                    }
                }
                else
                {
                    // FT_Write Failed
                    throw new IOException($"Failed to write device (error {ftStatus})");
                }
            }
            else
            {
                throw new IOException($"device is closed.");
            }
        }

        private int GetBytesToRead()
        {
            if (IsOpen)
            {
                uint rxQuene = 0;
                ftHandle!.GetRxBytesAvailable(ref rxQuene);
                if (ftStatus == FTDI.FT_STATUS.FT_OK)
                {
                    return (int)rxQuene;
                }
                else
                {
                    throw new IOException("Failed to get Queue Status (error " + ftStatus.ToString() + ")");
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        private int GetBytesToWrite()
        {
            if (IsOpen)
            {
                uint txQuene = 0;
                ftHandle!.GetTxBytesWaiting(ref txQuene);
                if (ftStatus == FTDI.FT_STATUS.FT_OK)
                {
                    return (int)txQuene;
                }
                else
                {
                    throw new IOException("Failed to get Queue Status (error " + ftStatus.ToString() + ")");
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }
}
