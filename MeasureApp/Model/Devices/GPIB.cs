using MeasureApp.Model.Common;
using NationalInstruments.VisaNS;
using System;
using System.Diagnostics;

namespace MeasureApp.Model.Devices
{
    public class GPIB : NotificationObjectBase
    {
        public bool IsOpen = false;
        public string deviceAddr = string.Empty;
        public MessageBasedSession messageBasedSession;

        public int Timeout
        {
            get => messageBasedSession.Timeout;
            set => messageBasedSession.Timeout = value;
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
                //MessageBox.Show(ex.ToString());
                Debug.WriteLine(ex.ToString());
                return Array.Empty<string>();
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
            return IsOpen ? messageBasedSession.Query(cmd) : null;
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

        public byte ReadStatusByte()
        {
            return (byte)messageBasedSession.ReadStatusByte();
        }
    }
}