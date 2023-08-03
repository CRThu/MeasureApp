using CarrotProtocolLib.Device;
using CarrotProtocolLib.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Driver
{
    public class FtdiD2xxDriver : IDriver
    {
        public bool IsOpen => throw new NotImplementedException();

        public int BytesToRead => throw new NotImplementedException();

        public int ReceivedByteCount => throw new NotImplementedException();

        public int SentByteCount => throw new NotImplementedException();

        public event SerialPortDriver.ErrorReceivedHandler? ErrorReceived;

        public void Close()
        {
            throw new NotImplementedException();
        }

        public DeviceInfo[] GetDevicesInfo()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public int Read(byte[] buffer, int offset, int bytesExpected)
        {
            throw new NotImplementedException();
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
