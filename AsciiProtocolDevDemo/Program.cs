using CarrotCommFramework.Protocols;
using CarrotCommFramework.Sessions;
using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;

namespace AsciiProtocolDevDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            var pipeReader = PipeReader.Create(
                Console.OpenStandardInput(),
                new StreamPipeReaderOptions(leaveOpen: true)
                );
            var pipeWriter = PipeWriter.Create(
                Console.OpenStandardOutput(),
                new StreamPipeWriterOptions(leaveOpen: true)
                );
            await ProcessMsgAsync(pipeReader, pipeWriter);
        }

        private static async Task ProcessMsgAsync(PipeReader reader, PipeWriter writer)
        {
            while (true)
            {
                ReadResult result = await reader.ReadAsync();
                ReadOnlySequence<byte> buffer = result.Buffer;
                Debug.WriteLine($"buffer.len={buffer.Length}");
                if (TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
                {
                    // Process the line.
                    string msg = $"Recv: {Encoding.ASCII.GetString(line)}\r\n";

                    await writer.WriteAsync(Encoding.ASCII.GetBytes(msg));
                    reader.AdvanceTo(buffer.Start);
                }
                else
                {
                    // Tell the PipeReader how much of the buffer has been consumed.

                    reader.AdvanceTo(buffer.Start, buffer.End);
                }


                // Stop reading if there's no more data coming.
                if (result.IsCompleted)
                {
                    break;
                }
            }

            // Mark the PipeReader as complete.
            await reader.CompleteAsync();
        }

        private static bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
        {
            // Look for a EOL in the buffer.
            SequencePosition? position = buffer.PositionOf((byte)'>');

            if (position == null)
            {
                line = default;
                return false;
            }

            // Skip the line + the \n.
            line = buffer.Slice(0, buffer.GetPosition(1, position.Value));
            buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
            return true;
        }
    }
}