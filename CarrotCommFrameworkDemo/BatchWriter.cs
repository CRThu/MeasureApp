using CarrotCommFramework.Protocols;
using CarrotCommFramework.Sessions;
using CarrotCommFramework.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFrameworkDemo
{
    public static class BatchWriter
    {
        public static void Run(Session s, string[] cmds, int delay = 100)
        {
            for (int i = 0; i < cmds.Length; i++)
            {
                Console.WriteLine(cmds[i]);
                s.Write(new((cmds[i] + "\r\n").AsciiToBytes()));
                if (cmds[i].Contains('?'))
                {
                    Console.WriteLine("WAITING");
                    byte[] rx = new byte[1024];
                    int len = s.Read(rx, 0, 1024);
                    Console.WriteLine($"{rx.Take(len).ToArray().BytesToAscii()}");
                }
                else
                {
                    Thread.Sleep(delay);
                }
            }
        }
        public static void RunFromFile(Session s, string filename, int delay = 100)
        {
            var cmds = File.ReadAllLines(filename);
            Run(s, cmds, delay);
        }
    }
}
