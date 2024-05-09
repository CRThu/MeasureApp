using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Loggers
{
    public class NLogLogger : ILogger
    {
        public string Name { get; set; }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
