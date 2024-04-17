using CarrotProtocolLib.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo
{
    public enum StreamStatus
    {
        Initial,
        Close,
        Open
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IDriverCommStream
    {
        public StreamStatus Status { get; set; }

        /// <summary>
        /// 获取可用设备列表
        /// </summary>
        /// <returns>可用设备信息数组</returns>
        public abstract static DeviceInfo[] GetDeviceInfos()
    }
}
