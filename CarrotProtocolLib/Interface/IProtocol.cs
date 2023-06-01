using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Interface
{
    public interface IProtocol
    {
        public delegate void ProtocolParseErrorHandler(Exception ex);

        public void Start();
        public void Stop();
        public void Send(byte[] bytes);
        public void Send(byte[] bytes, int offset, int length);
    }
}
