using System.Net.Sockets;
using System.Net;
using System.Net.Http;
using DryIoc;
using IOStreamDemo.Streams;
using IOStreamDemo.Drivers;
using IOStreamDemo.Loggers;
using IOStreamDemo.Sessions;

namespace IOStreamDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            // 注册资源
            SessionManager.Current.Container.RegisterInitializer<object>(
                            (anyObj, resolver) => Console.WriteLine($"Object {{{anyObj}}} Resolved."));
            DriverManager.RegisterResources(SessionManager.Current);
            LoggerManager.RegisterResources(SessionManager.Current);
            StreamManager.RegisterResources(SessionManager.Current);

            // 查找现有设备
            var deviceInfos = SessionManager.FindDevices(SessionManager.Current);
            foreach (var deviceInfo in deviceInfos)
            {
                Console.WriteLine($"{deviceInfo}");
            }

            // 创建Session
            CreateSession(SessionManager.Current, "COM://9@9600,8,N,1", "CONSOLE://1", "RAWV1");

            Console.WriteLine($"{SessionManager.Current.Sessions.First().Value.Stream}");
            Console.WriteLine($"{SessionManager.Current.Sessions.First().Value.Logger}");

        }

        public static Session CreateSession(SessionManager container, string address, string logger, string protocol)
        {

            // ADDRESS
            // COM://7@9600
            // TCP://127.0.0.1:8888
            // GPIB://22

            // LOGGER
            // CONSOLE://1

            // SERVICE
            // CDPV1

            string[] devInfo = address.ToUpper().Split("://", 2);
            string[] loggerInfo = logger.ToUpper().Split("://", 2);
            string protocolInfo = protocol.ToUpper();

            if (devInfo.Length != 2 || loggerInfo.Length != 2)
                throw new NotImplementedException();

            var resName = devInfo[0];
            var loggerName = loggerInfo[0];

            var s = container.Create(address, resName, loggerName, protocolInfo);

            return s;
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
