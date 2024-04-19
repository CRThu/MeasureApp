using System.Net.Sockets;
using System.Net;
using System.Net.Http;
using DryIoc;
using IOStreamDemo.Streams;
using IOStreamDemo.Drivers;
using IOStreamDemo.Loggers;

namespace IOStreamDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            SessionContainer sc = new();
            string loggerName = "ContainerLogger";

            // logger注册以及ioc模块日志创建object记录
            sc.container.Register<ILogger, ConsoleLogger>(Reuse.Singleton, serviceKey: loggerName);
            sc.container.RegisterInitializer<object>(
                (anyObj, resolver) => resolver.Resolve<ILogger>(loggerName).Log($"Object {{{anyObj}}} Resolved."),
                condition: request => !loggerName.Equals(request.ServiceKey));

            // 单例注册
            sc.container.Register<IDriver, SerialDriver>(serviceKey: "SerialDriver", reuse: Reuse.Singleton);
            sc.container.Register<IDriver, GpibDriver>(serviceKey: "GpibDriver", reuse: Reuse.Singleton);

            // 驱动通信流注册
            sc.container.Register<IDriverCommStream, SerialStream>(serviceKey: "SerialStream");
            sc.container.Register<IDriverCommStream, VisaGpibStream>(serviceKey: "VisaGpibStream");

            Console.WriteLine(sc.container.Resolve<IDriver>("SerialDriver").GetType().ToString());
            Console.WriteLine(sc.container.Resolve<IDriver>("GpibDriver").GetType().ToString());

            Console.WriteLine(sc.container.Resolve<IDriverCommStream>("SerialStream").GetType().ToString());
            Console.WriteLine(sc.container.Resolve<IDriverCommStream>("VisaGpibStream").GetType().ToString());


            //var a = DeviceManager.FindDevices("");
            //foreach (var device in a)
            //{
            //    Console.WriteLine(device.Name);
            //}

            //var session = DeviceManager.CreateSession("com://9", "console", "cdpv1");

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
        }
    }

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
