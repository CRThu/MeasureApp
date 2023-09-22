using CarrotProtocolLib.Device;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Service
{
    /// <summary>
    /// 设备数据接收服务类
    /// </summary>
    public class DeviceDataReceiveService : BaseTaskService<int>
    {
        /// <summary>
        /// 操作设备接口
        /// </summary>
        public IDevice? Device { get; set; }

        /// <summary>
        /// 接收缓冲区(内部使用)
        /// </summary>
        private byte[] ReceiveDataBuffer = new byte[16 * 1024 * 1024];

        public ulong Counter = 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DeviceDataReceiveService() : base()
        {
            Priority = ThreadPriority.Highest;
            TaskOptions = TaskCreationOptions.LongRunning;
        }

        /// <summary>
        /// 数据接收服务程序
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override int ServiceLoop()
        {
            /*
#if DEBUG
            var Logger = NLog.LogManager.Setup().LoadConfiguration(builder =>
            {
                //builder.ForLogger().WriteToDebug();
                builder.ForLogger().WriteToFile($"App_{DateTime.Now:yyyyMMddHHmmss}.log",
                    "${uppercase:${level}} | ${message}")
                    .WithAsync(NLog.Targets.Wrappers.AsyncTargetWrapperOverflowAction.Grow);
            }).GetCurrentClassLogger();

            Logger.Debug($"log start");

            //for (int i = 0; i < 10000; i++)
            //    Logger.Debug($"nlog test {DateTime.Now:HH:mm:ss.ffffff} : read = {0}, counter = {i}");
#endif
            */
            while (Device!.Driver!.IsOpen)
            {
                if (IsCancellationRequested)
                    return 1;

                int len = Device.Driver.BytesToRead;
                // Debug.WriteLine($"{DateTime.Now:HH:mm:ss.fff} : BytesToRead = {len}");

                if (len > ReceiveDataBuffer.Length)
                    len = ReceiveDataBuffer.Length;

                if (len > 0)
                {
                    int readBytes = Device.Driver.Read(ReceiveDataBuffer, 0, len);
                    if (readBytes != len)
                    {
                        throw new InvalidOperationException($"Read(): readBytes({readBytes}) != len({len}).");
                    }
                    // Debug.WriteLine($"BytesToRead = {len}, Read = {readBytes}, ReceivedByteCount = {ReceivedByteCount}");
                    //int len2 = Sp.BytesToRead;
                    //Debug.WriteLine($"BytesToRead2 = {len2}");
                    Device.RxBuffer.Write(ReceiveDataBuffer, 0, len);
                    Counter += (ulong)readBytes;
/*
#if DEBUG
                    Logger.Debug($"{DateTime.Now:HH:mm:ss.ffffff} : read = {readBytes}, counter = {Counter}");
#endif
*/
                    //Debug.WriteLine($"{DateTime.Now:HH:mm:ss.fff} : read = {readBytes}, counter = {Counter}");

                    // raw bytes
                    //byte[] rawBytesArr = new byte[len];
                    //Array.Copy(ReceiveDataBuffer, rawBytesArr, len);
                    //var bufBytesStr = BitConverter.ToString(rawBytesArr).Replace("-", "");
                    //File.AppendAllText("raw.txt", bufBytesStr);
                }
                else
                {
                    //Thread.Sleep(500);
                    //Debug.WriteLine($"{DateTime.Now:HH:mm:ss.fff} : sleep 500ms");
                }
            }
            return 0;
        }
    }
}
