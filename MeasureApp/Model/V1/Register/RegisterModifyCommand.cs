using MeasureApp.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.Register
{
    public class RegisterModifyCommand : NotificationObjectBase
    {
        public int Address { get; set; }
        public int BitsStart { get; set; }
        public int BitsLen { get; set; }
        public long Data { get; set; }

        public RegisterModifyCommand(int address, int bitsStart, int bitsLen, long data)
        {
            Address = address;
            BitsStart = bitsStart;
            BitsLen = bitsLen;
            Data = data;
        }
    }
}
