using CarrotProtocolLib.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Streams
{
    public class SerialStream : IDriverCommStream
    {
        public string Address { get; set; }
        public string LoggerKey { get; set; }
        /// <summary>
        /// 驱动层实现
        /// </summary>
        private SerialPort Driver { get; set; } = new();

        public void Config(string? cfg)
        {
            if (cfg == null)
                return;

            Driver = new SerialPort(cfg);
        }

        public void Close()
        {
            Driver.Close();
        }

        public void Open()
        {
            Driver.Open();
        }

        public void Write(ReadOnlySpan<byte> buffer)
        {
            Driver.BaseStream.Write(buffer);
        }

        public int Read(Span<byte> buffer)
        {
            return Driver.BaseStream.Read(buffer);
        }

    }
}
