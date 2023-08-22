using CarrotProtocolLib.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Logger
{
    public interface ILogger
    {
        public void Add(string from, string to, IProtocolFrame frame);
    }
}
