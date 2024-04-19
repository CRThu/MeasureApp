using CarrotProtocolLib.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Streams
{
    public class VisaGpibStream : IDriverCommStream
    {
        public DeviceInfo DeviceInfo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public StreamStatus Status { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ulong RxCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ulong TxCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event IDriverCommStream.StreamErrorReceivedHandler? StreamErrorReceived;

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Open(string addr)
        {
            throw new NotImplementedException();
        }

        public int Read(Span<byte> buffer, int offset, int bytesExpected)
        {
            throw new NotImplementedException();
        }

        public void Write(Span<byte> buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
