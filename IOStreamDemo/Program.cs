using System.Net.Sockets;
using System.Net;
using IOStreamDemo.Drivers;
using IOStreamDemo.Sessions;

namespace IOStreamDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            // 查找现有设备
            var deviceInfos = DriverFactory.Current.FindDevices();
            foreach (var deviceInfo in deviceInfos)
            {
                Console.WriteLine($"{deviceInfo}");
            }

            //var s = SessionFactory.Current.CreateSession(
            //    "S"
            //    + "+COM://COM250"
            //    + "+RAPV1://RAPV1"
            //    + "+CONSOLE://CONSOLE.1;NLOG://NLOG.1"
            //    + "+RECV;PARSE"
            //    , SessionConfig.Empty);
            
            var s = SessionFactory.Current.CreateSession(
                "SESSION1+COM://COM250"
                , SessionConfig.Default);

            s.Open();

            Console.WriteLine("WRITE...");
            s.Write("ABCDE");
            Console.WriteLine("RECV...");
            Console.WriteLine("PRESS ANY KEY TO EXIT");
            Console.ReadKey();
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
