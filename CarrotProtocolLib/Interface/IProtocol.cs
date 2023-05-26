using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Interface
{
    public interface IProtocol
    {
        public delegate void ReceiveErrorHandler(Exception ex);
        public event ReceiveErrorHandler ReceiveError;

        public void Start();
        public void Stop();
        public void Send(byte[] bytes);
        public void Send(byte[] bytes, int offset, int length);
    }
}
