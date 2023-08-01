using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Driver
{
    public class SerialPortDriver
    {
        private SerialPort Driver { get; set; }

        public bool IsOpen { get; set; }

        public int BytesToRead => Driver.BytesToRead;

        public SerialPortDriver()
        {
            Driver = new SerialPort();
        }

        public void Open()
        {

        }

        public void Close()
        {

        }

        public void Write(byte[] buffer, int offset, int count)
        {

        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return 0;
        }
    }
}
