using IOStreamDemo.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Loggers
{
    public class ConsoleLogger : ILogger
    {
        public string Name { get; set; }

        public void Config(string? cfg)
        {
            if (cfg == null)
                return;

        }

        public void Log(Packet packet)
        {
            Console.WriteLine($"{nameof(ConsoleLogger)}: " + packet.Message);
        }
    }
}
