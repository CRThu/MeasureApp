using CarrotCommFramework.Sessions;
using System.Net.Sockets;
using System.Net;
using CarrotCommFramework.Factory;
using CarrotCommFramework.Services;
using CarrotCommFramework.Loggers;
using DryIoc;
using CarrotCommFramework.Protocols;
using CarrotCommFramework.Streams;
using System.Diagnostics;
using CarrotCommFramework.Util;
using System.Text.Json.Serialization;

namespace CarrotCommFrameworkDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            //string testSession = "{" +
            //    "\"session\": [ { \"service\": \"session\", \"instance\": \"session1\" } ]," +
            //    "\"stream\": [ { \"service\": \"COM\", \"instance\": \"COM250\", \"baudrate\": \"115200\", \"databits\": \"8\", \"parity\": \"n\", \"stopbits\": \"1\" } ]," +
            //    "\"protocol\": [ { \"service\": \"RAPV1\", \"instance\": \"rapv1_inst1\" } ]," +
            //    "\"logger\": [ { \"service\": \"CONSOLE\", \"instance\": \"consolelogger_inst1\"} ]," +
            //    "\"service\": [ { \"service\": \"RECV\", \"instance\": \"recv_inst1\" }, { \"service\": \"PARSE\", \"instance\": \"parse_inst1\" } ]" +
            //    "}";

            string testSession = "{" +
                "\"session\": [ { \"service\": \"session\" } ]," +
                "\"stream\": [ { \"service\": \"VISA\", \"address\": \"TCPIP0::192.168.1.2::inst0::INSTR\", \"baudrate\": \"115200\", \"databits\": \"8\", \"parity\": \"n\", \"stopbits\": \"1\" } ]," +
                "\"protocol\": [ { \"service\": \"RAPV1\" } ]," +
                "\"logger\": [ { \"service\": \"CONSOLE\" } ]," +
                "\"service\": [ ]" +
                "}";

            var sc = SessionConfig.Create(testSession);
            foreach (var ll in sc.Components)
            {
                Console.WriteLine($"{ll.Type}::{ll.ServiceName}::{ll.InstanceName}::{SerializationHelper.SerializeToString(ll.Params)}");
            }
            return;

            // logger注册以及ioc模块日志创建object记录
            ProductProvider.Current.Container.RegisterInitializer<object>(
                (anyObj, resolver) => Console.WriteLine($"Object {{{anyObj}}} Resolved."));

            // 查找现有设备
            var deviceInfos = DriverFactory.Current.FindDevices();
            foreach (var deviceInfo in deviceInfos)
            {
                Console.WriteLine($"{deviceInfo}");
            }


            var s = SessionFactory.Current.CreateSession(
                testSession
                , SessionConfig.Empty);

            s.Open();

            Console.WriteLine("WRITE...");
            //s.Write(new("ABCDE".AsciiToBytes()));
            BatchWriter.RunFromFile(s, "D:\\Projects\\NB2408\\testcmd.txt", 100);
            byte[] rx = new byte[1024];
            for (int i = 0; i < 16; i++)
            {
                ///// INT
                //s.Write(new("*CLS\r\n".AsciiToBytes()));
                //Thread.Sleep(100);
                //s.Write(new("*TRG\r\n".AsciiToBytes()));
                //Thread.Sleep(100);
                //while (true)
                //{
                //    s.Write(new("*STB?\r\n".AsciiToBytes()));
                //    int len = s.Read(rx, 0, 1024);
                //    Console.WriteLine($"{rx.Take(len).ToArray().BytesToAscii().Replace("\n", "")}");
                //    if (len > 3)
                //        break;
                //    Thread.Sleep(100);
                //}
            }
            Console.WriteLine("COMPLETE REQUEST...");
            s.Close();

            Console.WriteLine("EXIT.");

            //DataRecvService service = new();
            //service.Run(serialStream, new RawAsciiProtocol());

            //byte[] b = [0x01, 0x02, 0x03, 0x04, 0x05];
            //s.Stream.Write(b, 0, 5);
            //Thread.Sleep(5000);
            //byte[] rdBuf = new byte[256];
            //var readLen = s.Stream.Read(rdBuf, 0, rdBuf.Length);
            //Console.WriteLine($"{readLen}");
            //s.Stream.Close();
        }
    }

    /*
    Console.WriteLine("Press any key to start reading data...");
    Console.ReadKey();
    TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888));

    string ipAddress = "127.0.0.1"; // Example IP Address
    int port = 13000; // Example Port

    CustomProgress<int> progressReporter = new CustomProgress<int>(bytesReceived => Console.WriteLine($"Progress: Received {bytesReceived} bytes of data."));
    TcpStreamHandler tcpStreamHandler = new TcpStreamHandler(ipAddress, port, progressReporter);

    CancellationTokenSource cts = new CancellationTokenSource();
    Task readTask = tcpStreamHandler.StartReadingAsync(cts.Token);

    Console.WriteLine("Press any key to stop reading data...");
    Console.ReadKey();

    cts.Cancel();
    try
    {
        readTask.Wait(cts.Token); // Ensure any cleanup or final operations are completed
    }
    catch (Exception e)
    {
        if (e is OperationCanceledException)
        {
            Console.WriteLine($"Reading stopped. Total packets received: {tcpStreamHandler.TotalPacketsReceived}");
        }
    }
    */
    public class TcpStreamHandler
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private int totalBytesRead = 0;
        private readonly CustomProgress<int> progress;
        public int TotalPacketsReceived { get; private set; } = 0;

        public TcpStreamHandler(string ipAddress, int port, CustomProgress<int> progress)
        {
            tcpClient = new TcpClient();
            tcpClient.ReceiveBufferSize = 1048576;
            IPEndPoint ipEp = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            tcpClient.Connect(ipEp);
            networkStream = tcpClient.GetStream();
            this.progress = progress;
        }

        public async Task StartReadingAsync(CancellationToken cancellationToken)
        {
            try
            {
                byte[] buffer = new byte[4096];
                while (!cancellationToken.IsCancellationRequested)
                {
                    int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    if (bytesRead > 0)
                    {
                        totalBytesRead += bytesRead;
                        progress?.Report(totalBytesRead);
                        UnpackData(buffer, bytesRead);
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private int UnpackData(byte[] data, int bytesRead)
        {
            int packets = bytesRead / 2;
            TotalPacketsReceived += bytesRead;
            for (int i = 0; i < packets; i++)
            {
                byte[] packet = new byte[2];
                Array.Copy(data, i * 2, packet, 0, 2);
                // Process each packet here
                //Console.WriteLine($"Packet {i + 1}/{packets}: {BitConverter.ToString(packet)}");
            }
            return packets;
        }
    }

    public class CustomProgress<T> : IProgress<T>
    {
        private readonly Action<T> action;

        public CustomProgress(Action<T> action)
        {
            this.action = action;
        }

        public void Report(T value)
        {
            action?.Invoke(value);
        }
    }
}
