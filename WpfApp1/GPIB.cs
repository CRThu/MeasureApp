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
            return ResourceManager.GetLocalManager().FindResources(expression);
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

        public void Write(string cmd)
        {
            if (isOpen)
            {
                messageBasedSession.Write(cmd);
            }
        }

        public string Query(string cmd)
        {
            return isOpen ? messageBasedSession.Query(cmd).Trim() : null;
        }
    }
}