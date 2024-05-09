using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Loggers
{
    public interface ILogger
    {
        public string Name { get; set; }
        public void Log(string message);
    }
}
