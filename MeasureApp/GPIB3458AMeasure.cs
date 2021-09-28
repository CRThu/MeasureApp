using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp
{
    public class GPIB3458AMeasure
    {
        private GPIB gpib = new();
        public bool IsOpen => gpib.IsOpen;

        public int Timeout
        {
            get => gpib.Timeout;
            set => gpib.Timeout = value;
        }

        public string Open(string deviceAddr)
        {
            gpib.Open(deviceAddr);
            Timeout = 5000;
            gpib.Write("END");
            return gpib.Query("ID?");
        }

        public void Dispose()
        {
            gpib.Dispose();
        }

        public string QueryCommand(string text)
        {
            return gpib.Query(text);
        }

        public void WriteCommand(string text)
        {
            gpib.Write(text);
        }

        public string ReadString()
        {
            return gpib.ReadString();
        }

        // 3458A Multimeter User's Guide Page 149
        public decimal ReadVoltageSync()
        {
            // TODO
            return 0M;
        }
    }
}
