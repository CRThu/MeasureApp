using CarrotProtocolLib.Device;
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
        private IDevice? Device { get; set; }

        /// <summary>
        /// 接收缓冲区(内部使用)
        /// </summary>
        private byte[] ReceiveDataBuffer = new byte[64 * 1024];

        /// <summary>
        /// 构造函数
        /// </summary>
        public DeviceDataReceiveService() : base()
        {
        }

        /// <summary>
        /// 数据接收服务程序
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override int ServiceLoop()
        {
            while (Device!.Driver!.IsOpen)
            {
                if (IsCancellationRequested)
                    return 1;

                int len = Device.Driver.BytesToRead;

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
                    Debug.WriteLine($"buflen = {ReceiveDataBuffer.Length}");
                }
                else
                {
                    Thread.Sleep(5);
                }
            }
            return 0;
        }
    }
}
