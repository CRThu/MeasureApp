﻿using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Threading.Channels;
using System.Text;
using CarrotProtocolLib.Driver;
using CarrotProtocolLib.Device;
using CarrotProtocolLib.Protocol;
using CarrotProtocolLib.Util;
using System.Diagnostics;
using System.IO;

namespace CarrotProtocolLibBenchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //FileStream s = new FileStream("test.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, true);

            //DeviceInfo? dev = FtdiD2xxDriver.GetDevicesInfo().FirstOrDefault();
            //if (dev is null)
            //{
            //    Console.WriteLine("Device not found");
            //    return;
            //}

            //FtdiD2xxStream s = new FtdiD2xxStream(dev.Name);
            TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888));
            listener.Start();
            TcpClient tcpClient = listener.AcceptTcpClient();
            NetworkStream s = tcpClient.GetStream();

            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Run(() => Write(s, cts));
            Task.Run(() => Read(s, cts));
            Console.ReadKey();
            cts.Cancel();
            s.Close();
        }

        public static void Write(Stream s, CancellationTokenSource cts)
        {
            BinaryWriter bw = new BinaryWriter(s);

            while (!cts.Token.IsCancellationRequested)
            {
                byte[] w = new byte[256];
                //for (int i = 0; i < w.Length; i++)
                //    w[i] = (byte)i;
                w = SetSampleControl(1).ToBytes();
                //bw.Write(w);
                bw.Write(w);
                bw.Flush();
                Console.WriteLine($"Write {w.Length} Bytes to stream");
                Task.Delay(1000, cts.Token).Wait(cts.Token);
            }
        }

        public static void Read(Stream s, CancellationTokenSource cts)
        {
            BinaryReader br = new BinaryReader(s);
            byte[] b = new byte[1024 * 1024];
            int b_len = 1024 * 1024;
            int readLen = 0;
            while ((readLen = br.Read(b, 0, b_len)) > 0)
            {
                //Console.WriteLine(Encoding.UTF8.GetString(b, 0, readLen));
                //Console.WriteLine(BitConverter.ToString(b, 0, readLen).Replace("-", " "));
                Console.WriteLine($"Read {readLen} Bytes from stream");
            }
        }

        public static CarrotDataProtocolFrame SetSampleControl(int start)
        {
            byte[] payload = new byte[16];
            byte[] RwnBytes = 0.IntToBytes();
            byte[] RegfileBytes = 0.IntToBytes();
            byte[] AddressBytes = 0x6.IntToBytes();
            byte[] ValueBytes = start.IntToBytes(); ;
            Array.Copy(RwnBytes, 0, payload, 0, 4);
            Array.Copy(RegfileBytes, 0, payload, 4, 4);
            Array.Copy(AddressBytes, 0, payload, 8, 4);
            Array.Copy(ValueBytes, 0, payload, 12, 4);
            CarrotDataProtocolFrame rec = new CarrotDataProtocolFrame(0xA0, 0, payload);
            Debug.WriteLine($"Send {nameof(CarrotDataProtocolFrame)}: {rec.FrameBytes.BytesToHexString()}");
            return rec;
        }
    }
}