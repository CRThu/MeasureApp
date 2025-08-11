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
        public DataLogValue(UInt32 value)
        {
            Type = ValueType.UInt64;
            UInt64 = value;
        }
        public DataLogValue(UInt16 value)
        {
            Type = ValueType.UInt64;
            UInt64 = value;
        }
        public DataLogValue(byte value)
        {
            Type = ValueType.UInt64;
            UInt64 = value;
        }

        public DataLogValue(Int64 value)
        {
            Type = ValueType.Int64;
            Int64 = value;
        }

        public DataLogValue(Int32 value)
        {
            Type = ValueType.Int64;
            Int64 = value;
        }

        public DataLogValue(Int16 value)
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
                UInt64 u => new DataLogValue(u),
                UInt32 u32 => new DataLogValue(u32),
                UInt16 u16 => new DataLogValue(u16),
                byte u8 => new DataLogValue(u8),

                Int64 i => new DataLogValue(i),
                Int32 i32 => new DataLogValue(i32),
                Int16 i16 => new DataLogValue(i16),
                sbyte i8 => new DataLogValue(i8),

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
        //private static readonly Lazy<DispatcherTimer> _sharedTimer = new Lazy<DispatcherTimer>(() =>
        //{
        //    var timer = new DispatcherTimer();
        //    timer.Interval = TimeSpan.FromMicroseconds(100);
        //    timer.Tick += SharedTimer_Tick;
        //    Application.Current.Dispatcher.Invoke(() => timer.Start());
        //    return timer;
        //}, LazyThreadSafetyMode.ExecutionAndPublication);
        private static readonly Lazy<System.Timers.Timer> _sharedTimer = new Lazy<System.Timers.Timer>(() =>
        {
            var timer = new System.Timers.Timer(100);
            timer.Elapsed += SharedTimer_Tick;
            timer.AutoReset = true;
            timer.Start();
            return timer;
        }, LazyThreadSafetyMode.ExecutionAndPublication);

        private static readonly ConcurrentBag<WeakReference<DataLogList>> _activeInstances = new ConcurrentBag<WeakReference<DataLogList>>();

        private readonly ConcurrentQueue<DataLogValue> _quene = new ConcurrentQueue<DataLogValue>();
        private bool _disposed = false;

        public readonly List<DataLogValue> items = new List<DataLogValue>();
        public IReadOnlyList<DataLogValue> Items => items.AsReadOnly();

        public DataLogList()
        {
            var _ = _sharedTimer.Value;
            _activeInstances.Add(new WeakReference<DataLogList>(this));
        }

        private static void SharedTimer_Tick(object sender, EventArgs e)
        {
            foreach (var weakRef in _activeInstances)
            {
                if (weakRef.TryGetTarget(out var instance) && !instance._disposed)
                {
                    instance.ProcessQuene();
                }
            }
        }

        private void ProcessQuene()
        {
            bool hasChanges = false;
            while (_quene.TryDequeue(out var dataLogValue))
            {
                items.Add(dataLogValue);
                hasChanges = true;
            }
            if (hasChanges)
            {
                // 手动触发更新
                OnPropertyChanged(nameof(Items));
            }
        }

        public void Add<T>(T value) => _quene.Enqueue(DataLogValue.From<T>(value));

        public void AddRange<T>(IEnumerable<T> values)
        {
            foreach (var value in values)
                _quene.Enqueue(DataLogValue.From<T>(value));
        }

        public void AddRange<T>(ReadOnlySpan<T> values)
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

        //public IEnumerable<string> Keys => logs.Keys;
        [ObservableProperty]
        private ObservableCollection<string> keys = new ObservableCollection<string>();
        private readonly object _keysLock = new object();

        public bool Contains(string key) => logs.ContainsKey(key);

        public DataLogList this[string key] => GetOrAddKey(key);

        public DataLogList GetOrAddKey(string key)
        {
            if (logs.TryGetValue(key, out var v1))
            {
                return v1;
            }

            lock (_keysLock)
            {
                if (logs.TryGetValue(key, out var v2))
                {
                    return v2;
                }

                if (logs.TryAdd(key, new DataLogList()))
                {
                    Application.Current.Dispatcher.Invoke(() => Keys.Add(key));
                    // notify when update key
                    //OnPropertyChanged(nameof(Keys));
                }
            }
            return logs[key];
        }

        public void RemoveKey(string key)
        {
            lock (_keysLock)
            {
                if (logs.TryRemove(key, out var l))
                {
                    l.Dispose();
                    Application.Current.Dispatcher.Invoke(() => Keys.Remove(key));
                    // notify when remove key
                    //OnPropertyChanged(nameof(Keys));
                }
            }
        }

        public void Clear()
        {
            lock (_keysLock)
            {
                foreach (var log in logs.Values)
                {
                    log.Dispose();
                }
                logs.Clear();
                Application.Current.Dispatcher.Invoke(() => Keys.Clear());
                // notify when remove key
                //OnPropertyChanged(nameof(Keys));
            }
        }

        public void Add<T>(string key, T value)
        {
            GetOrAddKey(key).Add<T>(value);
        }

        public void AddRange<T>(string key, IEnumerable<T> value)
        {
            GetOrAddKey(key).AddRange<T>(value);
        }

        public void HandlePacket(IPacket packet)
        {
            if (packet.PacketType == PacketType.Data)
            {
                var pkt = packet as IDataPacket;
                foreach (var key in pkt.Keys)
                {
                    // and key with sender
                    int ch = Convert.ToInt32(key);

                    // TODO channel
                    switch ((pkt.Type, pkt.Encoding))
                    {
                        case (DataType.INT64, DataEncoding.TwosComplement):
                            GetOrAddKey(key).AddRange(pkt.Get<Int64>(ch));
                            break;
                        case (DataType.INT64, DataEncoding.OffsetBinary):
                            GetOrAddKey(key).AddRange(pkt.Get<UInt64>(ch));
                            break;
                        case (DataType.INT32, DataEncoding.TwosComplement):
                            GetOrAddKey(key).AddRange(pkt.Get<Int32>(ch));
                            break;
                        case (DataType.INT32, DataEncoding.OffsetBinary):
                            GetOrAddKey(key).AddRange(pkt.Get<UInt32>(ch));
                            break;
                        case (DataType.INT16, DataEncoding.TwosComplement):
                            GetOrAddKey(key).AddRange(pkt.Get<Int16>(ch));
                            break;
                        case (DataType.INT16, DataEncoding.OffsetBinary):
                            GetOrAddKey(key).AddRange(pkt.Get<UInt16>(ch));
                            break;
                        case (DataType.INT8, DataEncoding.TwosComplement):
                            GetOrAddKey(key).AddRange(pkt.Get<sbyte>(ch));
                            break;
                        case (DataType.INT8, DataEncoding.OffsetBinary):
                            GetOrAddKey(key).AddRange(pkt.Get<byte>(ch));
                            break;
                        case (DataType.FP64, _):
                            GetOrAddKey(key).AddRange(pkt.Get<double>(ch));
                            break;
                        default:
                            throw new NotSupportedException($"Type {pkt.Type} is not supported");
                    }
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
