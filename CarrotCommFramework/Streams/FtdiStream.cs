using HighPrecisionTimer;
using CarrotCommFramework.Util;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FTD2XX_NET;
using CarrotCommFramework.Drivers;
using static FTD2XX_NET.FTDI;
using System.Diagnostics;
using DryIoc;
using System.Net.NetworkInformation;
using System.Threading;

namespace CarrotCommFramework.Streams
{
    public class FtdiStream : StreamBase
    {
        /// <summary>
        /// FTDI驱动包装类
        /// </summary>
        private FTDI Ftdi { get; set; }

        private string SerialNumber { get; set; }

        private int Timeout { get; set; } = 1000;

        private byte FtdiMask { get; set; }
        private byte FtdiMode { get; set; }

        public FtdiStream()
        {
        }

        /// <summary>
        /// 流指示有数据
        /// </summary>
        public override bool ReadAvailable => Ftdi != null && Ftdi.IsOpen && (GetBytesToRead() != 0);

        /// <summary>
        /// 配置解析和初始化
        /// </summary>
        /// <param name="params"></param>
        public override void Config(IDictionary<string, string> @params = default!)
        {
            if (@params.Count == 0)
                return;

            Ftdi = new FTDI();

            if (@params.Count > 0)
                SerialNumber = @params["id"];

            //if (@params.Length > 1)
            //    Driver.BaudRate = Convert.ToInt32(@params[1]);
            //if (@params.Length > 2)
            //    Driver.DataBits = Convert.ToInt32(@params[2]);
            //if (@params.Length > 3)
            //    Driver.Parity = SerialPortHelper.ParityString2Enum(@params[3]);
            //if (@params.Length > 4)
            //    Driver.StopBits = SerialPortHelper.StopBitsFloat2Enum(Convert.ToDouble(@params[4]));

            // TODO TIMEOUT/MASK/MODE
        }

        /// <summary>
        /// 关闭流
        /// </summary>
        public override void Close()
        {
            Ftd2xxNetDecorator.Ftd2xxNetWrapper(() => Ftdi.Close());
        }

        /// <summary>
        /// 打开流
        /// </summary>
        public override void Open()
        {
            Ftd2xxNetDecorator.Ftd2xxNetWrapper(() => Ftdi.OpenBySerialNumber(SerialNumber));

            // Set Timeout
            Ftd2xxNetDecorator.Ftd2xxNetWrapper(() => Ftdi.SetTimeouts((uint)Timeout, (uint)Timeout));

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
            Ftd2xxNetDecorator.Ftd2xxNetWrapper(() => Ftdi.SetBitMode(mask, mode));
        }

        /// <summary>
        /// 流写入
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteInternal(buffer, offset, count);
        }

        /// <summary>
        /// 流读取
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return ReadInternal(buffer, offset, count);
        }

        private void WriteInternal(byte[] buffer, int offset, int count)
        {
            byte[] bufferWithZeroOffset = buffer;
            uint numBytesWritten = 0;
            if (offset != 0)
            {
                bufferWithZeroOffset = buffer.Skip(offset).ToArray();
            }

            Ftd2xxNetDecorator.Ftd2xxNetWrapper(() => Ftdi.Write(bufferWithZeroOffset, count, ref numBytesWritten));

            // Waiting for transfer done
            // TODO 同步流写入存在阻塞，待优化
            while (count != numBytesWritten)
            {
                Debug.WriteLine($"Waiting for write device done ({numBytesWritten}/{count})");
            }
        }

        private int ReadInternal(byte[] buffer, int offset, int bytesExpected)
        {
            uint currentBytesRead = 0;
            uint currentBytesExpected = 0;
            int totalBytesRead = 0;
            byte[] rx = new byte[Math.Min(bytesExpected, 1048576)];

            int BytesToRead = GetBytesToRead();
            bytesExpected = BytesToRead > bytesExpected ? bytesExpected : BytesToRead;

            while (bytesExpected > 0)
            {
                // 一次读取不超过读取缓冲区的长度字节流
                currentBytesExpected = (uint)bytesExpected;
                currentBytesExpected = rx.Length > currentBytesExpected ? currentBytesExpected : (uint)rx.Length;

                // Read
                Ftd2xxNetDecorator.Ftd2xxNetWrapper(() => Ftdi.Read(rx, currentBytesExpected, ref currentBytesRead));

                // Copy to Buffer
                Array.Copy(rx, 0, buffer, offset, currentBytesRead);

                offset += (int)currentBytesRead;
                totalBytesRead += (int)currentBytesRead;
                bytesExpected -= (int)currentBytesRead;
            }
            return totalBytesRead;
        }

        private int GetBytesToRead()
        {
            uint rxQuene = 0;
            Ftd2xxNetDecorator.Ftd2xxNetWrapper(() => Ftdi.GetRxBytesAvailable(ref rxQuene));
            return (int)rxQuene;
        }
    }
}
