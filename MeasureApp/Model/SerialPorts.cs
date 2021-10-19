using MeasureApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MeasureApp.Model
{
    public class SerialPorts : NotificationObjectBase
    {
        private ObservableDictionary<string, SerialPort> serialPortsDict = new();
        public ObservableDictionary<string, SerialPort> SerialPortsDict
        {
            get => serialPortsDict;
            set
            {
                serialPortsDict = value;
                RaisePropertyChanged(() => SerialPortsDict);
            }
        }

        public IEnumerable<string> SerialPortNames => SerialPortsDict.Keys;
        public IEnumerable<SerialPort> SerialPortInstances => SerialPortsDict.Values;

        public SerialPorts()
        {
        }

        public bool Open(string portName, int baudRate = 115200, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            if (!SerialPortsDict.ContainsKey(portName))
            {
                SerialPort serialPort = new(portName)
                {
                    BaudRate = baudRate,
                    Parity = parity,
                    DataBits = dataBits,
                    StopBits = stopBits,
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

        public int Read(string serialPort, byte[] responseBytes, int bytesExpected)
        {
            return Read(serialPort, responseBytes, bytesExpected, SerialPortsDict[serialPort].ReadTimeout);
        }

        public int Read(string serialPort, byte[] responseBytes, int bytesExpected, int millisecondsTimeout)
        {
            int offset = 0, bytesRead;
            bool result = Utility.TimeoutCheck(millisecondsTimeout, () =>
             {
                 while (bytesExpected > 0)
                 {
                     if (SerialPortsDict[serialPort].BytesToRead > 0)
                     {
                         try
                         {
                             bytesRead = SerialPortsDict[serialPort].Read(responseBytes, offset, bytesExpected);
                             offset += bytesRead;
                             bytesExpected -= bytesRead;
                         }
                         catch (Exception)
                         {
                         }
                     }
                 }
                 return true;
             });
            return offset;
        }
    }
}
