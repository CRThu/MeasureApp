using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Driver
{
    public static class SerialPortHelper
    {
        public static StopBits StopBitsFloat2Enum(float stopBits)
        {
            return stopBits switch
            {
                0f => StopBits.None,
                1f => StopBits.One,
                2f => StopBits.Two,
                1.5f => StopBits.OnePointFive,
                _ => throw new DriverErrorException(nameof(SerialPortHelper), $"不支持的停止位: {stopBits:0.0}"),
            };
        }

        public static Parity ParityString2Enum(string parity)
        {
            return parity switch
            {
                "None" => Parity.None,
                "Odd" => Parity.Odd,
                "Even" => Parity.Even,
                "Mark" => Parity.Mark,
                "Space" => Parity.Space,
                _ => throw new DriverErrorException(nameof(SerialPortHelper), $"不支持的校验位: {parity}"),
            };
        }
    }
}
