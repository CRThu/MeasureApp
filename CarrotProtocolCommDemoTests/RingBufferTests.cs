using CarrotProtocolLib.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolCommDemo.Tests
{
    [TestClass()]
    public class RingBufferTests
    {
        [TestMethod()]
        public void RingBufferTest()
        {
            RingBuffer rb = new(16);
            byte[] a = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            byte[] b = new byte[] { 33, 44, 55, 66, 77, 88 };
            byte[] c = new byte[] { 123, 234 };
            byte[] d = new byte[] { 12, 34, 56, 78, 90 };
            byte[] a_exp1 = new byte[a.Length];
            byte[] a_exp2 = new byte[a.Length];
            byte[] b_exp1 = new byte[b.Length];
            byte[] b_exp2 = new byte[b.Length];
            byte[] c_exp1 = new byte[c.Length];
            byte[] c_exp2 = new byte[c.Length];
            byte[] c_exp3 = new byte[c.Length];
            byte[] c_exp4 = new byte[c.Length];
            byte[] d_exp1 = new byte[d.Length];
            byte[] d_exp2 = new byte[d.Length];
            byte[] d_exp3 = new byte[d.Length];
            byte[] d_exp4 = new byte[d.Length];
            rb.Write(a);
            rb.Write(b);
            rb.Read(a_exp1);
            rb.Read(b_exp1);
            rb.Write(c);
            rb.Read(c_exp1);
            rb.Write(d);
            rb.Read(d_exp1);
            rb.Write(c);
            rb.Write(a);
            rb.Read(c_exp2);
            rb.Read(a_exp2);
            rb.Write(d);
            rb.Write(b);
            rb.Read(d_exp2);
            rb.Read(b_exp2);
            rb.Write(d);
            rb.Write(c);
            rb.Write(c);
            rb.Write(d);
            rb.Read(d_exp3);
            rb.Read(c_exp3);
            rb.Read(c_exp4);
            rb.Read(d_exp4);

            AssertArrayEqual(a, a_exp1);
            AssertArrayEqual(a, a_exp2);
            AssertArrayEqual(b, b_exp1);
            AssertArrayEqual(b, b_exp2);
            AssertArrayEqual(c, c_exp1);
            AssertArrayEqual(c, c_exp2);
            AssertArrayEqual(c, c_exp3);
            AssertArrayEqual(c, c_exp4);
            AssertArrayEqual(d, d_exp1);
            AssertArrayEqual(d, d_exp2);
            AssertArrayEqual(d, d_exp3);
            AssertArrayEqual(d, d_exp4);
        }

        public void AssertArrayEqual<T>(T exp, T act)
        {
            Assert.AreEqual(string.Join(',', exp), string.Join(',', act));
        }
    }
}