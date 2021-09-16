using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class SerialPorts
    {
        public Dictionary<string, SerialPort> SerialPortsDict = new Dictionary<string, SerialPort>();

        public IEnumerable<string> SerialPortNames => SerialPortsDict.Select(SerialPortList => SerialPortList.Key);

        public SerialPorts()
        {
        }


        public bool Open(string portName, int baudRate = 115200)
        {
            if (!SerialPortsDict.ContainsKey(portName))
            {
                SerialPort serialPort = new SerialPort(portName)
                {
                    BaudRate = baudRate,
                    ReadBufferSize = Properties.Settings.Default.SerialPortReadBufferSize,
                    WriteBufferSize = Properties.Settings.Default.SerialPortWriteBufferSize,
                    ReadTimeout = Properties.Settings.Default.SerialPortReadTimeout,
                    WriteTimeout = Properties.Settings.Default.SerialPortWriteTimeout
                };
                serialPort.Open();
                SerialPortsDict.Add(portName, serialPort);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Close(string portName)
        {
            if (SerialPortsDict.ContainsKey(portName))
            {
                SerialPortsDict[portName].Close();
                _ = SerialPortsDict.Remove(portName);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CloseAll()
        {
            foreach (KeyValuePair<string, SerialPort> serialPort in SerialPortsDict.ToArray())
            {
                serialPort.Value.Close();
                _ = SerialPortsDict.Remove(serialPort.Key);
            }
        }
    }
}
