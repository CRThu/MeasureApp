using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Streams.Protocols
{
    public class RawAsciiProtocolStream : IProtocol
    {
        public string Version { get; } = "RAPV1";

        public bool ReadAvailable => true;

        public void Open()
        {

        }

        public void Close()
        {

        }


        public int Read(byte[] buffer, int offset, int count)
        {
            return 0;
        }

        public void Write(byte[] buffer, int offset, int count)
        {

        }
    }
}
