using MeasureApp.Model.Common;
using MeasureApp.Model.Converter;
using MeasureApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MeasureApp.Model.Devices
{
    public class SerialPorts : NotificationObjectBase
    {
        // 写入串口前触发委托
        public delegate void PreWriteStringHandler(string portName, string data);
        public event PreWriteStringHandler PreWriteString;

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

        public bool Open(string portName, int baudRate = 115200, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One, AppConfig appConfig = null)
        {
            appConfig ??= new();

            if (!SerialPortsDict.ContainsKey(portName))
            {
                SerialPort serialPort = new(portName)
                {
                    BaudRate = baudRate,
                    Parity = parity,
                    DataBits = dataBits,
                    StopBits = stopBits,
                    ReadBufferSize = appConfig.Device.SerialPort.Buffer,
                    WriteBufferSize = appConfig.Device.SerialPort.Buffer,
                    ReadTimeout = appConfig.Device.SerialPort.Timeout,
                    WriteTimeout = appConfig.Device.SerialPort.Timeout
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
            if (PreWriteString is not null)
                PreWriteString(serialPort, strData);
            //SerialPortsDict[serialPort].Write(strData);
            SerialPortsDict[serialPort].WriteLine(strData);
        }

        public void WriteBytes(string serialPort, byte[] buffer)
        {
            SerialPortsDict[serialPort].Write(buffer, 0, buffer.Length);
        }

        public string ReadExistingString(string serialPort)
        {
            return Encoding.Default.GetString(ReadExistingBytes(serialPort));
        }

        public byte[] ReadExistingBytes(string serialPort)
        {
            int len = SerialPortsDict[serialPort].BytesToRead;
            byte[] buf = new byte[len];
            ReadBytes(serialPort, buf, len);
            return buf;
        }

        public string ReadLine(string serialPort)
        {
            return SerialPortsDict[serialPort].ReadLine();
        }

        public byte[] GetDataPacket(string serialPort, string cmd, int pktLen)
        {
            byte[] cmdBytes = BytesConverter.String2Bytes(cmd);
            return GetDataPacket(serialPort, cmdBytes, pktLen);
        }

        public byte[] GetDataPacket(string serialPort, byte[] cmd, int pktLen)
        {
            WriteBytes(serialPort, cmd);

            byte[] buf = new byte[pktLen];
            int pktBytesRead = ReadBytes(serialPort, buf, pktLen);

            if (pktBytesRead != pktLen)
                throw new Exception($"GetDataPacket() Received: {pktBytesRead}/{pktLen}.");

            return buf;
        }

        public int ReadBytesWithTimeoutCheck(string serialPort, byte[] responseBytes, int bytesExpected)
        {
            return ReadBytesWithTimeoutCheck(serialPort, responseBytes, bytesExpected, SerialPortsDict[serialPort].ReadTimeout);
        }

        // 线程不安全
        public int ReadBytesWithTimeoutCheck(string serialPort, byte[] responseBytes, int bytesExpected, int millisecondsTimeout)
        {
            int offset = 0, bytesRead;
            bool result = Utility.TimeoutCheck(millisecondsTimeout, () =>
            {
                while (bytesExpected > 0)
                {
                    try
                    {
                        bytesRead = SerialPortsDict[serialPort].Read(responseBytes, offset, bytesExpected);
                        offset += bytesRead;
                        bytesExpected -= bytesRead;
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
                return true;
            });
            return offset;
        }

        public int ReadBytes(string serialPort, byte[] responseBytes, int bytesExpected)
        {
            int offset = 0, bytesRead;
            while (bytesExpected > 0)
            {
                bytesRead = SerialPortsDict[serialPort].Read(responseBytes, offset, bytesExpected);
                offset += bytesRead;
                bytesExpected -= bytesRead;
            }
            return offset;
        }

        public void RemoveDataReceivedEvent(string serialPort, SerialDataReceivedEventHandler serialDataReceivedEventHandler)
        {
            SerialPortsDict[serialPort].DataReceived -= serialDataReceivedEventHandler;
        }

        public void AddDataReceivedEvent(string serialPort, SerialDataReceivedEventHandler serialDataReceivedEventHandler)
        {
            SerialPortsDict[serialPort].DataReceived += serialDataReceivedEventHandler;
        }
    }
}
