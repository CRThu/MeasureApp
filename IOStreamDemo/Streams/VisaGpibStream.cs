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
        public string Address { get; set; }
        public string LoggerKey { get; set; }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Open(string addr)
        {
            throw new NotImplementedException();
        }

        public int Read(string s)
        {
            throw new NotImplementedException();
        }

        public void Write(string s)
        {
            throw new NotImplementedException();
        }
    }
}
