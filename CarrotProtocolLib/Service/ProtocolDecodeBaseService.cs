using CarrotProtocolLib.Device;
using CarrotProtocolLib.Protocol;
using CarrotProtocolLib.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Service
{
    public class ProtocolDecodeBaseService : BaseTaskService<int>
    {
        /// <summary>
        /// 操作设备接口
        /// </summary>
        public IDevice? Device { get; set; }

        /// <summary>
        /// 数据帧存储接口
        /// </summary>
        public ILogger Logger { get; set; }

        public delegate void ProtocolDecodeErrorHandler(Exception ex);
        public event ProtocolDecodeErrorHandler ProtocolDecodeError;

        public ProtocolDecodeBaseService() : base()
        {
            Priority = ThreadPriority.Lowest;
            TaskOptions = TaskCreationOptions.LongRunning;
        }
    }

}
