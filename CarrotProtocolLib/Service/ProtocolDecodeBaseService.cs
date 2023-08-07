using CarrotProtocolLib.Device;
using CarrotProtocolLib.Impl;
using CarrotProtocolLib.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Service
{
    public class ProtocolDecodeBaseService : BaseTaskService<int>
    {
        public delegate void ProtocolDecodeErrorHandler(Exception ex);
        public event ProtocolDecodeErrorHandler ProtocolDecodeError;

        public ProtocolDecodeBaseService() : base()
        {
        }
    }

}
