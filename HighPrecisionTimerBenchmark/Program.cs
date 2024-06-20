using HdrHistogram;
using HighPrecisionTimer;
using System.Diagnostics;
using System.Linq;

namespace HighPrecisionTimerBenchmark
{
    internal class Program
    {
        static int i = 0;
        static int count = 100000;

        static void Main(string[] args)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            Task t = RunTimer();

            while (true)
            {
                if (t.IsCompleted)
                    break;
                else
                {
                    Thread.Sleep(100);
                    Console.WriteLine($"{i}/{count}, pos.");
                }
            }

            Console.WriteLine("EXIT.");
        }

        static async Task RunTimer()
        {
            // run
            long start = Stopwatch.GetTimestamp();
            for (i = 0; i < count; i++)
            {

                HighPrecisionTimer.HighPrecisionTimer timer = new((uint)1);
                TaskCompletionSource taskCompletionSource = new();
                timer.Tick += (object? sender, TickEventArgs e) =>
                {
                    timer.Stop();
                    taskCompletionSource.SetResult();
                };
                timer.Start();
                await taskCompletionSource.Task;
            }
            long end = Stopwatch.GetTimestamp();

            long elapsedNs = (end - start) * 1000L * 1000L * 1000L / Stopwatch.Frequency;
            Console.WriteLine($"Average Elapsed: {elapsedNs / count / 1000.0:F2} us.");
        }

        static void RunHist()
        {
            int tick = 0;
            long[] list = new long[1048576];
            int[] pid = new int[1048576];
            var hist = new LongHistogram(TimeStamp.Hours(1), 3);

            HighPrecisionTimer.HighPrecisionTimer timer = new(100);
            timer.Tick += (object? sender, TickEventArgs e) =>
            {
                list[tick] = Stopwatch.GetTimestamp();
                tick++;
                pid[tick] = Environment.CurrentManagedThreadId;
            };

            // pre-run
            timer.Start();
            Thread.Sleep(1000);
            timer.Stop();

            // measure
            tick = 0;
            timer.Start();
            Thread.Sleep(10000);
            timer.Stop();

            Console.WriteLine($"thread id: {Environment.CurrentManagedThreadId}");
            Console.WriteLine($"Tick Count: {tick}");

            for (int i = 1; i < tick; i++)
            {
                long elapsedNs = (list[i] - list[i - 1]) * 1000L * 1000L * 1000L / Stopwatch.Frequency;
                hist.RecordValue(elapsedNs);

                Console.WriteLine($"thread id: {pid[i]}");
            }

            using var writer = new StreamWriter("Hist.hgrm");
            hist.OutputPercentileDistribution(writer);
        }
    }
}
