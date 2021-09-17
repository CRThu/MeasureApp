using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class SerialPorts
    {
        public Dictionary<string, SerialPort> SerialPortsDict = new Dictionary<string, SerialPort>();

        public IEnumerable<string> SerialPortNames => SerialPortsDict.Select(SerialPortList => SerialPortList.Key);
        public IEnumerable<SerialPort> SerialPortInstances => SerialPortsDict.Select(SerialPortList => SerialPortList.Value);

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

        public void WriteString(string serialPort, string strData)
        {
            SerialPortsDict[serialPort].Write(strData);
        }

        public void WriteBytes(string serialPort, byte[] buffer, int bytesCount)
        {
            SerialPortsDict[serialPort].Write(buffer, 0, bytesCount);
        }

        public string ReadExistingString(string serialPort)
        {
            return SerialPortsDict[serialPort].ReadExisting();
        }

        public void ReadExistingBytes(string serialPort, byte[] responseBytes)
        {
            int offset = 0, bytesRead;

            while (SerialPortsDict[serialPort].BytesToRead > 0)
            {
                bytesRead = SerialPortsDict[serialPort].Read(responseBytes, offset, SerialPortsDict[serialPort].BytesToRead);
                offset += bytesRead;
            }
        }

        // SOLUTION:https://stackoverflow.com/questions/16439897/serialport-readbyte-int32-int32-is-not-blocking-but-i-want-it-to-how-do
        // return Success or Receive whole packet timeout
        public int Read(string serialPort, byte[] responseBytes, int bytesExpected)
        {
            int offset = 0, bytesRead;
            Thread t = new Thread(o => Thread.Sleep(SerialPortsDict[serialPort].ReadTimeout));
            t.Start(this);
            while (t.IsAlive)
            {
                if (bytesExpected > 0 && SerialPortsDict[serialPort].BytesToRead > 0)
                {
                    bytesRead = SerialPortsDict[serialPort].Read(responseBytes, offset, bytesExpected);
                    offset += bytesRead;
                    bytesExpected -= bytesRead;
                }
                else if (bytesExpected == 0)
                {
                    t.Abort();
                }
            }
            return offset;
        }

    }
}
