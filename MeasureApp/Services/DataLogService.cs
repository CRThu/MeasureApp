using CarrotLink.Core.Logging;
using CarrotLink.Core.Protocols.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MeasureApp.Services
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
        [FieldOffset(4)] public readonly UInt64 UInt64;
        [FieldOffset(4)] public readonly Int64 Int64;
        [FieldOffset(4)] public readonly double Double;

        public DataLogValue()
        {
            Type = ValueType.Null;
        }

        public DataLogValue(UInt64 value)
        {
            Type = ValueType.UInt64;
            UInt64 = value;
        }

        public DataLogValue(Int64 value)
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

        public static DataLogValue FromUInt64(UInt64 value) => new DataLogValue(value);
        public static DataLogValue FromInt64(Int64 value) => new DataLogValue(value);
        public static DataLogValue FromDouble(double value) => new DataLogValue(value);

        public static DataLogValue From<T>(T value)
        {
            return value switch
            {
                UInt64 u => new DataLogValue(u),
                Int64 i => new DataLogValue(i),
                Double d => new DataLogValue(d),
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

    public partial class DataLogList : ObservableObject, IDisposable
    {
        private readonly ConcurrentQueue<DataLogValue> _quene = new ConcurrentQueue<DataLogValue>();
        private readonly DispatcherTimer _timer;
        private bool _disposed = false;

        public readonly List<DataLogValue> items = new List<DataLogValue>();
        public IReadOnlyList<DataLogValue> Items => items.AsReadOnly();

        public DataLogList()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMicroseconds(100);
            _timer.Tick += ProcessQuene;
            _timer.Start();
        }

        private void ProcessQuene(object sender, EventArgs e)
        {
            while (_quene.TryDequeue(out var dataLogValue))
            {
                items.Add(dataLogValue);
            }

            // 手动触发更新
            OnPropertyChanged(nameof(Items));
        }

        public void Add<T>(T value) => _quene.Enqueue(DataLogValue.From<T>(value));

        public void AddRange<T>(IEnumerable<T> values)
        {
            foreach (var value in values)
                _quene.Enqueue(DataLogValue.From<T>(value));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Tick -= ProcessQuene;
                    //_timer = null;
                }
            }

            _disposed = true;
        }

        ~DataLogList()
        {
            Dispose(false);
        }
    }

    public partial class DataLogService : ObservableObject, IPacketLogger, IDisposable
    {
        private readonly ConcurrentDictionary<string, DataLogList> logs = new ConcurrentDictionary<string, DataLogList>();
        private bool _disposed = false;

        public IEnumerable<string> Keys => logs.Keys;

        public DataLogList this[string key] => logs.GetOrAdd(key, vf =>
        {
            var l = new DataLogList();
            // notifty when update key
            OnPropertyChanged(nameof(Keys));
            return l;
        });

        public void Add<T>(string key, T value)
        {
            this[key].Add<T>(value);
        }

        public void AddRange<T>(string key, IEnumerable<T> value)
        {
            this[key].AddRange<T>(value);
        }

        public void RemoveKey(string key)
        {
            logs.TryRemove
        }

        public void HandlePacket(IPacket packet)
        {
            if (packet.PacketType == PacketType.Data)
            {
                var pkt = packet as IDataPacket;
                foreach (var channel in pkt.Channels)
                {
                    // TODO datatype and key with sender
                    var vals = pkt.GetValues<UInt64>(channel);
                    this[channel.ToString()].AddRange(vals);
                }
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                if (logs != null)
                {
                    foreach (var log in logs.Values)
                    {
                        log.Dispose();
                    }
                }
            }

            _disposed = true;
        }

        ~DataLogService()
        {
            Dispose(false);
        }
    }
}
