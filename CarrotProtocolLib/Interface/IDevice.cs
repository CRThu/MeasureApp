using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Interface
{
    public interface IDevice
    {
        public int ReceivedByteCount { get; }
        public int SentByteCount { get; }

        public bool IsOpen { get; }
        public int RxByteToRead { get; }

        public void Open();
        public void Close();
        public void Write(byte[] bytes);
        public void Read(byte[] responseBytes, int offset, int bytesExpected);

        public static abstract DeviceInfo[] GetDevicesInfo();

        public delegate void OnInternalPropertyChangedHandler(string name, dynamic value);
        public event OnInternalPropertyChangedHandler InternalPropertyChanged;
    }

    public enum InterfaceType
    {
        SerialPort,
        FTDI_D2XX,
    }

    public class DeviceInfo
    {
        public InterfaceType Interface { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{Interface} | {Name} | {Description}";
        }

        public DeviceInfo(InterfaceType @interface, string name, string description)
        {
            Interface = @interface;
            Name = name;
            Description = description;
        }
    }
}
