using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Driver
{
    [Serializable]
    public class DriverErrorException : Exception
    {
        public DriverErrorException()
        {
        }

        public DriverErrorException(string message, Exception? innerException = null)
            : base($"[ERROR]::{message}", innerException)
        {

        }

        public DriverErrorException(object sender, string message, Exception? innerException = null)
            : base($"[ERROR]::{nameof(sender)}::{message}", innerException)
        {

        }
        public DriverErrorException(string sender, string message, Exception? innerException = null)
            : base($"[ERROR]::{sender}::{message}", innerException)
        {

        }
    }
}
