using CarrotProtocolLib.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Streams
{
    public class VisaGpibStream : IDriverCommStream
    {
        public string Address { get; set; }
        public string LoggerKey { get; set; }

        public void Config(string? cfg)
        {
            if (cfg == null)
                return;

        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public void Write(ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public int Read(Span<byte> buffer)
        {
            throw new NotImplementedException();
        }
    }
}
