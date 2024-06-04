using CarrotCommFramework.Factory;
using CarrotCommFramework.Loggers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace CarrotCommFramework.Util
{
    public class TraceHelper
    {
        public static void Forward(Stream stream, bool autoFlush = true)
        {
            //var tcpClient = new TcpClient();
            //tcpClient.ReceiveBufferSize = 1048576;
            //IPEndPoint ipEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 13000);
            //tcpClient.Connect(ipEp);
            //var networkStream = tcpClient.GetStream();
            //Trace.Listeners.Add(new TextWriterTraceListener(networkStream, "127.0.0.1:13000"));
            Trace.Listeners.Add(new TextWriterTraceListener(stream, nameof(TraceHelper)));
            Trace.AutoFlush = autoFlush;
            Debug.AutoFlush = autoFlush;
        }
    }
}
