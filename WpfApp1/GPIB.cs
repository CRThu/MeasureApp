using NationalInstruments.VisaNS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public class GPIB
    {
        public bool IsOpen = false;
        private bool _isDataAvailable = false;
        private bool _isReadyForInstructions = false;
        public string deviceAddr = string.Empty;
        public MessageBasedSession messageBasedSession;

        public bool IsDataAvailable
        {
            get
            {
                _isDataAvailable = IsOpen && (((short)messageBasedSession.ReadStatusByte() & 128) != 0);
                return _isDataAvailable;
            }
        }

        public bool IsReadyForInstructions
        {
            get
            {
                _isReadyForInstructions = IsOpen && (((short)messageBasedSession.ReadStatusByte() & 16) != 0);
                return _isReadyForInstructions;
            }
        }

        public GPIB()
        {
        }

        public GPIB(string deviceAddr)
        {
            this.deviceAddr = deviceAddr;
        }


        // Search Devices
        // Reference: documentation.help/VISA.NET/VISA_Attibutes.htm
        // INSTR: Instrument Control
        // INTFC: GPIB Bus Interface
        public static string[] SearchDevices(string expression = "GPIB?*INSTR")
        {
            try
            {
                return ResourceManager.GetLocalManager().FindResources(expression);
            }
            catch (Exception ex)
            {
                // _ = MessageBox.Show(ex.ToString());
                return new string[] { };
            }
        }

        public void Open()
        {
            Open(deviceAddr);
        }

        public void Open(string deviceAddr)
        {
            messageBasedSession = (MessageBasedSession)ResourceManager.GetLocalManager().Open(deviceAddr);
            IsOpen = true;
            this.deviceAddr = deviceAddr;
        }

        public void Dispose()
        {
            IsOpen = false;
            deviceAddr = string.Empty;
            if (messageBasedSession != null)
            {
                messageBasedSession.Dispose();
            }
        }

        public string Query(string cmd)
        {
            return IsOpen ? messageBasedSession.Query(cmd).Trim() : null;
        }

        public decimal QueryDemical(string cmd)
        {
            return ConvertNumber(Query(cmd));
        }

        public void Write(string cmd)
        {
            if (IsOpen)
            {
                messageBasedSession.Write(cmd);
            }
        }

        public string ReadString()
        {
            return IsOpen ? messageBasedSession.ReadString().Trim() : null;
        }

        public decimal ReadDemical()
        {
            return ConvertNumber(ReadString());
        }

        public static decimal ConvertNumber(string data)
        {
            return Convert.ToDecimal(decimal.Parse(data, System.Globalization.NumberStyles.Float));
        }

        public void WaitForDataAvailable()
        {
            while (!IsDataAvailable)
            {
                Thread.Sleep(50);
            }
        }
    }
}