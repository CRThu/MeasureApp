using CarrotProtocolLib.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Interface
{
    public interface IDevice
    {
        /// <summary>
        /// 接收数据字节数
        /// </summary>
        public int ReceivedByteCount { get; }
        /// <summary>
        /// 发送数据字节数
        /// </summary>
        public int SentByteCount { get; }

        /// <summary>
        /// 设备是否打开
        /// </summary>
        public bool IsOpen { get; }

        /// <summary>
        /// 缓冲区待接收的数据字节数
        /// </summary>
        public int RxByteToRead { get; }

        /// <summary>
        /// 设备打开
        /// </summary>
        public void Open();

        /// <summary>
        /// 设备关闭
        /// </summary>
        public void Close();

        /// <summary>
        /// 设备写入字节流
        /// </summary>
        /// <param name="bytes"></param>
        public void Write(byte[] bytes);

        /// <summary>
        /// 设备读取字节流存储到数组位置
        /// </summary>
        /// <param name="responseBytes">接收数据存储数组</param>
        /// <param name="offset">偏移量</param>
        /// <param name="bytesExpected">请求读取数量</param>
        public void Read(byte[] responseBytes, int offset, int bytesExpected);

        /// <summary>
        /// 读取设备信息
        /// </summary>
        /// <returns></returns>
        public static abstract DeviceInfo[] GetDevicesInfo();

        /// <summary>
        /// 设备类内部属性更新
        /// </summary>
        /// <param name="name">属性名称</param>
        /// <param name="value">属性值</param>
        public delegate void DevicePropertyChangedHandler(string name, dynamic value);
        /// <summary>
        /// 设备类内部属性更新事件
        /// </summary>
        public event DevicePropertyChangedHandler DevicePropertyChanged;
    }
    
}
