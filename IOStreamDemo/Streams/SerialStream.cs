using CarrotProtocolLib.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Streams
{
    public class SerialStream : IDriverCommStream, IAsyncStream
    {
        public string Address { get; set; }
        public string LoggerKey { get; set; }

        public bool ReadAvailable => Driver.BytesToRead > 0;
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

        public void Write(byte[] buffer, int offset, int count)
        {
            Driver.BaseStream.Write(buffer, offset, count);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return Driver.BaseStream.Read(buffer, offset, count);
        }

        public Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return 0;

                    if (ReadAvailable)
                        return Read(buffer, offset, count);
                }
            },cancellationToken);
        }
    }
}
