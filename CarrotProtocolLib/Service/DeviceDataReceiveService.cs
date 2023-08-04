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
        private IDevice Device { get; set; }
        private byte[] ReceiveDataBuffer = new byte[65536];

        public DeviceDataReceiveService(IDevice device) : base()
        {
            Device = device;
        }

        public override int ServiceLoop()
        {
            while (Device.Driver.IsOpen)
            {
                if (IsCancellationRequested)
                    return 1;

                int len = Device.Driver.BytesToRead;
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
                    if (ReceiveDataBuffer.Length > 0)
                    {
                        Device.RxBuffer.Write(ReceiveDataBuffer, 0, len);
                        Debug.WriteLine($"buflen = {ReceiveDataBuffer.Length}");
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            return 0;
        }
    }
}
