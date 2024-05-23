using CarrotProtocolLib.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Streams
{
    public class VisaGpibStream : IStream
    {
        public string Address { get; set; }
        public string Name { get; set; }

        public VisaGpibStream(string name)
        {
            Name = name;
        }


        public bool ReadAvailable { get; set; } = true;

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

        public void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
