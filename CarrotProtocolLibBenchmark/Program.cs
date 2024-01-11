using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CarrotProtocolLib.Util;

namespace RingBufferBenchmark
{
    // A class to run the benchmark tests
    public class RingBufferTest
    {
        // A parameter to specify the size of the data to write and read
        [Params(4, 16, 64, 256, 1024)]
        public int DataSize { get; set; }

        // A parameter to specify the mode of the channel
        [Params(ChannelMode.Bounded, ChannelMode.Unbounded)]
        public ChannelMode ChannelMode { get; set; }

        // A method to write data to the ring buffer and channel
        [Benchmark]
        public async Task WriteDataAsync()
        {
            // Create a ring buffer to store and pass the data
            var rb = new RingBuffer(DataSize * 50);

            // Create a channel to store and pass the data
            var channel = ChannelMode == ChannelMode.Bounded ?
                Channel.CreateBounded<byte[]>(100) :
                Channel.CreateUnbounded<byte[]>();

            // Generate some random data
            var data = new byte[DataSize];
            new Random().NextBytes(data);

            // Write the data to the ring buffer
            rb.Write(data, 0, DataSize);

            // Write the data to the channel
            await channel.Writer.WriteAsync(data);

            // Complete the channel writer
            channel.Writer.Complete();
        }

        // A method to read data from the ring buffer and channel
        [Benchmark]
        public async Task ReadDataAsync()
        {
            // Create a ring buffer to store and pass the data
            var rb = new RingBuffer(DataSize * 50);

            // Create a channel to store and pass the data
            var channel = ChannelMode == ChannelMode.Bounded ?
                Channel.CreateBounded<byte[]>(100) :
                Channel.CreateUnbounded<byte[]>();

            // Generate some random data
            var data = new byte[DataSize];
            new Random().NextBytes(data);

            // Write the data to the ring buffer
            rb.Write(data, 0, DataSize);

            // Write the data to the channel
            await channel.Writer.WriteAsync(data);

            // Complete the channel writer
            channel.Writer.Complete();

            // Read data from the ring buffer
            rb.Read(data, 0, DataSize);

            // Read data from the channel
            await channel.Reader.ReadAsync();
        }
    }

    // An enum to specify the mode of the channel
    public enum ChannelMode
    {
        Bounded,
        Unbounded
    }

    // The main program
    class Program
    {
        static void Main(string[] args)
        {
            // Run the benchmark tests
            var summary = BenchmarkRunner.Run<RingBufferTest>();
        }
    }
}

