using NationalInstruments.VisaNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public class GPIB
    {
        public bool isOpen = false;
        public string deviceAddr = string.Empty;
        public MessageBasedSession messageBasedSession;

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
                _ = MessageBox.Show(ex.ToString());
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
            isOpen = true;
            this.deviceAddr = deviceAddr;
        }

        public void Dispose()
        {
            isOpen = false;
            deviceAddr = string.Empty;
            if (messageBasedSession != null)
            {
                messageBasedSession.Dispose();
            }
        }

        public string Query(string cmd)
        {
            return isOpen ? messageBasedSession.Query(cmd).Trim() : null;
        }

        public decimal QueryDemical(string cmd)
        {
            return ConvertNumber(Query(cmd));
        }

        public void Write(string cmd)
        {
            if (isOpen)
            {
                messageBasedSession.Write(cmd);
            }
        }

        public string ReadString()
        {
            return isOpen ? messageBasedSession.ReadString().Trim() : null;
        }

        public static decimal ConvertNumber(string data)
        {
            Console.WriteLine(data);
            return Convert.ToDecimal(decimal.Parse(data, System.Globalization.NumberStyles.Float));
        }
    }
}