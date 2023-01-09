using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrotProtocolLib.Impl;

namespace CarrotProtocolLib.Interface
{
    public interface ILogger
    {
        public void AddRx(CarrotDataProtocol protocol);
        public void AddTx(CarrotDataProtocol protocol);
    }
}
