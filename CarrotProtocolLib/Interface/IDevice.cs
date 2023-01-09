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
    }
}
