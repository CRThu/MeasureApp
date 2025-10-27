using CarrotLink.Core.Utility;

namespace MeasureApp.Model.Log
{
    public record CommandLogEntry
    {
        public DateTime TimeStamp { get; init; }
        public string Sender { get; init; }
        public string Message { get; init; }
        public string FriendlyMessage => Message.ToEscapeString();

        public override string ToString()
        {
            return $"[{TimeStamp:yyyy-MM-dd hh:mm:ss.fff}] {Sender}: {FriendlyMessage}";
        }
    }
}
