using System.Runtime.InteropServices;

namespace MeasureApp.Model.Log
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct DataLogValue
    {
        public enum ValueType : byte
        {
            Null = 0,
            UInt64 = 1,
            Int64 = 2,
            Double = 3,
        }

        [FieldOffset(0)] public readonly ValueType Type;
        [FieldOffset(4)] public readonly ulong UInt64;
        [FieldOffset(4)] public readonly long Int64;
        [FieldOffset(4)] public readonly double Double;

        public DataLogValue()
        {
            Type = ValueType.Null;
        }

        public DataLogValue(ulong value)
        {
            Type = ValueType.UInt64;
            UInt64 = value;
        }
        public DataLogValue(uint value)
        {
            Type = ValueType.UInt64;
            UInt64 = value;
        }
        public DataLogValue(ushort value)
        {
            Type = ValueType.UInt64;
            UInt64 = value;
        }
        public DataLogValue(byte value)
        {
            Type = ValueType.UInt64;
            UInt64 = value;
        }

        public DataLogValue(long value)
        {
            Type = ValueType.Int64;
            Int64 = value;
        }

        public DataLogValue(int value)
        {
            Type = ValueType.Int64;
            Int64 = value;
        }

        public DataLogValue(short value)
        {
            Type = ValueType.Int64;
            Int64 = value;
        }

        public DataLogValue(sbyte value)
        {
            Type = ValueType.Int64;
            Int64 = value;
        }

        public DataLogValue(double value)
        {
            Type = ValueType.Double;
            Double = value;
        }

        public static DataLogValue Null => new DataLogValue();

        public static DataLogValue From<T>(T value)
        {
            return value switch
            {
                ulong u => new DataLogValue(u),
                uint u32 => new DataLogValue(u32),
                ushort u16 => new DataLogValue(u16),
                byte u8 => new DataLogValue(u8),

                long i => new DataLogValue(i),
                int i32 => new DataLogValue(i32),
                short i16 => new DataLogValue(i16),
                sbyte i8 => new DataLogValue(i8),

                double d => new DataLogValue(d),
                _ => throw new NotSupportedException($"Type {typeof(T)} is not supported")
            };
        }

        public string GetValueString()
        {
            return Type switch
            {
                ValueType.Null => "<null>",
                ValueType.UInt64 => UInt64.ToString(),
                ValueType.Int64 => Int64.ToString(),
                ValueType.Double => Double.ToString("G"),
                _ => "<invalid>"
            };
        }

        public override string ToString()
        {
            return $"[{Type}] {GetValueString()}";
        }
    }
}
