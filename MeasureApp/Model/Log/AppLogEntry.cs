using CarrotLink.Core.Logging;

namespace MeasureApp.Model.Log
{
    public record AppLogEntry
    {
        public DateTime TimeStamp { get; init; }
        public LogLevel LogLevel { get; init; }
        public string Message { get; init; }

        public override string ToString()
        {
            return $"[{TimeStamp:yyyy-MM-dd hh:mm:ss.fff}] {LogLevel.ToString().ToUpper()}: {Message}";
        }
    }
}
