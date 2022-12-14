using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolCommDemo
{
    public interface ILogger
    {
        public void AddRx(CarrotDataProtocol protocol);
        public void AddTx(CarrotDataProtocol protocol);
    }
}
