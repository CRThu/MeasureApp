using Microsoft.VisualStudio.TestTools.UnitTesting;
using CarrotProtocolCommDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolCommDemo.Tests
{
    [TestClass()]
    public class CarrotDataProtocolTests
    {
        [TestMethod()]
        public void CarrotDataProtocolTest()
        {
            byte[] bytes = new byte[] { 0x3C, 0x30, 0x22, 0x11, 0x08, 0x05, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x00, 0xEE, 0xEE, 0x3E };
            CarrotDataProtocol carrotDataProtocol = new(bytes, 0, bytes.Length);
            Assert.AreEqual(0x3C, carrotDataProtocol.FrameStart);
            Assert.AreEqual(0x1122, carrotDataProtocol.ControlFlags);
            Assert.AreEqual(0x08, carrotDataProtocol.StreamId);
            Assert.AreEqual(0x0005, carrotDataProtocol.PayloadLength);
            Assert.AreEqual(string.Join(' ', new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55 }), string.Join(' ', carrotDataProtocol.Payload));
            Assert.AreEqual(0xEEEE, carrotDataProtocol.Crc16);
            Assert.AreEqual(0x3E, carrotDataProtocol.FrameEnd);

            byte[] bytes2 = carrotDataProtocol.ToBytes();
            Assert.AreEqual(string.Join(' ', bytes), string.Join(' ', bytes2));
        }

        [TestMethod()]
        public void CrcTest()
        {
            byte[] bytes = new byte[] { 0x3C, 0x30, 0x00, 0x00, 0x00, 0x02, 0x00, 0x0D, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x3C, 0x87, 0x3E };
            CarrotDataProtocol carrotDataProtocol = new(bytes, 0, bytes.Length);
            int crc = carrotDataProtocol.GenerateCrc();
            Assert.AreEqual(0x873C,crc);
            Assert.IsFalse(carrotDataProtocol.CheckCrcError());
        }
    }
}