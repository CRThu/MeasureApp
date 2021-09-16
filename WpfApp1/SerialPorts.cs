using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class SerialPorts
    {
        public List<SerialPort> SerialPortList = new List<SerialPort>();

        public IEnumerable<string> SerialPortNames => SerialPortList.Select(SerialPortList => SerialPortList.PortName);

        public SerialPorts()
        {
        }

        public void Add(string portName)
        {
            SerialPortList.Add(new SerialPort(portName));
        }
    }
}
