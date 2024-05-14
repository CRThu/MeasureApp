using HdrHistogram;
using HighPrecisionTimer;
using System.Diagnostics;

namespace HighPrecisionTimerBenchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            int tick = 0;
            long[] list = new long[1048576];
            var hist = new LongHistogram(TimeStamp.Hours(1), 3);

            HighPrecisionTimer.HighPrecisionTimer timer = new(1);
            timer.Tick += (object? sender, TickEventArgs e) =>
            {
                list[tick] = Stopwatch.GetTimestamp();
                tick++;
            };

            // pre-run
            timer.Start();
            Thread.Sleep(10000);
            timer.Stop();

            // measure
            tick = 0;
            timer.Start();
            Thread.Sleep(60000);
            timer.Stop();

            Console.WriteLine($"Tick Count: {tick}");

            for (int i = 1; i < tick; i++)
            {
                long elapsedNs = (list[i] - list[i - 1]) * 1000L * 1000L * 1000L / Stopwatch.Frequency;
                hist.RecordValue(elapsedNs);
            }

            using var writer = new StreamWriter("Hist.hgrm");
            hist.OutputPercentileDistribution(writer);
        }
    }
}
