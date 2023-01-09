using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrotProtocolLib.Util;

namespace CarrotProtocolCommDemo.Tests
{
    [TestClass()]
    public class BytesExTests
    {
        [TestMethod()]
        public void HexStringToBytesTest()
        {
            string str1 = "1234567890AABBCCDDEEFF";
            byte[] bytes1 = BytesEx.HexStringToBytes(str1);
            byte[] bytes2 = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff };
            Assert.AreEqual(string.Join(' ', bytes2), string.Join(' ', bytes1));
        }

        [TestMethod()]
        public void BytesToHexStringTest()
        {
            byte[] bytes1 = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff };
            string str1 = BytesEx.BytesToHexString(bytes1);
            string str2 = "12 34 56 78 90 AA BB CC DD EE FF";
            Assert.AreEqual(str2, str1);
        }

        [TestMethod()]
        public void BytesToAsciiTest()
        {
            byte[] bytes1 = new byte[] { 0x30, 0x31, 0x32, 0x33, 0x34, 0x41, 0x42, 0x43, 0x44, 0x45 };
            string str1 = BytesEx.BytesToAscii(bytes1);
            string str2 = "01234ABCDE";
            Assert.AreEqual(str2, str1);
        }

        [TestMethod()]
        public void AsciiToBytesTest()
        {
            string str1 = "01234ABCDE";
            byte[] bytes1 = BytesEx.AsciiToBytes(str1);
            byte[] bytes2 = new byte[] { 0x30, 0x31, 0x32, 0x33, 0x34, 0x41, 0x42, 0x43, 0x44, 0x45 };
            Assert.AreEqual(string.Join(' ', bytes2), string.Join(' ', bytes1));
        }
    }
}