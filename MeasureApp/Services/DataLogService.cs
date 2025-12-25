using CarrotLink.Core.Logging;
using CarrotLink.Core.Protocols.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using MeasureApp.Model.Log;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MeasureApp.Services
{

    public partial class DataLogList : ObservableObject, IDisposable
    {
        private readonly Channel<DataLogValue> _channel;

        // 内部存储数据
        private readonly List<DataLogValue> _items = new List<DataLogValue>();
        private readonly object _itemsLock = new object();

        // 消费者任务
        private readonly Task _consumerTask;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private bool _disposed = false;

        // UI 刷新间隔 (毫秒)
        private const int UpdateIntervalMs = 50;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Summary))]
        private DataLogValue latestValue;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Summary))]
        private int length;

        public string Summary
        {
            get
            {
                if (LatestValue.Type == DataLogValue.ValueType.Null)
                    return "<NULL>";
                if (Length > 1)
                {
                    // 数组模式: [type](Length)
                    return $"[{LatestValue.Type}]({Length})";
                }
                // 标量模式: [type] Value
                return LatestValue.ToString();
            }
        }

        public IReadOnlyList<DataLogValue> Items
        {
            get
            {
                lock (_itemsLock)
                {
                    return _items.AsReadOnly();
                }
            }
        }

        public DataLogList()
        {
            var options = new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false, // 允许多个线程写入
                AllowSynchronousContinuations = false
            };
            _channel = Channel.CreateUnbounded<DataLogValue>(options);

            _consumerTask = Task.Run(() => ProcessQueueAsync(_cts.Token));
        }

        /// <summary>
        /// 消费者循环：从 Channel 读取数据 -> 缓冲 -> 定时批量更新 UI
        /// </summary>
        private async Task ProcessQueueAsync(CancellationToken token)
        {
            var buffer = new List<DataLogValue>();
            var reader = _channel.Reader;

            try
            {
                while (await reader.WaitToReadAsync(token))
                {
                    // 1. 尽可能多地从 Channel 读取数据到临时 buffer
                    while (reader.TryRead(out var item))
                    {
                        buffer.Add(item);
                    }

                    if (buffer.Count > 0)
                    {
                        // 2. 将数据拷贝并在ui线程更新
                        var batchData = buffer.ToArray();
                        var lastItem = batchData[^1];
                        buffer.Clear();

                        // 3. 通知 UI 更新
                        Application.Current?.Dispatcher.InvokeAsync(() =>
                        {
                            lock (_itemsLock)
                            {
                                _items.AddRange(batchData);
                                // 属性状态维护
                                LatestValue = lastItem;
                                Length = _items.Count;
                            }

                            OnPropertyChanged(nameof(Items));
                        }, DispatcherPriority.Background, token);
                    }

                    // 4. 避免 UI 刷新过于频繁
                    await Task.Delay(UpdateIntervalMs, token);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in DataLogList consumer task: {ex}");
            }
        }

        public void Add<T>(T value)
        {
            _channel.Writer.TryWrite(DataLogValue.From(value));
        }

        public void AddRange<T>(IEnumerable<T> values)
        {
            foreach (var v in values)
            {
                _channel.Writer.TryWrite(DataLogValue.From(v));
            }
        }

        public void AddRange<T>(ReadOnlySpan<T> values)
        {
            foreach (var v in values)
            {
                _channel.Writer.TryWrite(DataLogValue.From(v));
            }
        }

        /// <summary>
        /// 提供当前数据的线程安全快照。
        /// </summary>
        public DataLogValue[] GetSnapshot()
        {
            lock (_itemsLock)
            {
                return _items.ToArray();
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
                _cts.Cancel();
                _channel.Writer.TryComplete();
                try
                { _consumerTask.Wait(500); }
                catch
                { }
                _cts.Dispose();
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


        public bool TryGetValue(string key, out DataLogList? dataLogList)
        {
            if (key == null)
            {
                dataLogList = null;
                return false;
            }

            return logs.TryGetValue(key, out dataLogList);
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

        /// <summary>
        /// 将指定key的数据复制到系统剪贴板。
        /// </summary>
        /// <param name="key"></param>
        public void CopyToClipboard(string key)
        {
            if (string.IsNullOrEmpty(key) || !logs.TryGetValue(key, out var dataLogList))
                return;

            var dataSnapshot = dataLogList.GetSnapshot();

            if (dataSnapshot.Length == 0)
            { 
                // Nothing to copy.
                return;
            }

            var sb = new StringBuilder();
            foreach (var value in dataSnapshot)
            {
                sb.AppendLine(value.GetValueString());
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    Clipboard.SetText(sb.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error copying to clipboard: {ex}");
                }
            });
        }

        public void Add<T>(string key, T value)
        {
            GetOrAddKey(key).Add<T>(value);
        }

        public void AddRange<T>(string key, IEnumerable<T> value)
        {
            GetOrAddKey(key).AddRange<T>(value);
        }

        public void HandlePacket(IPacket packet, string from, string to)
        {
            if (packet.PacketType == PacketType.Data)
            {
                var pkt = packet as IDataPacket;
                foreach (var key in pkt.Keys)
                {
                    // and key with sender
                    var list = GetOrAddKey($"{from}->{to}" + "." + key);

                    // TODO channel
                    switch ((pkt.Type, pkt.Encoding))
                    {
                        case (DataType.INT64, DataEncoding.TwosComplement):
                            list.AddRange(pkt.Get<Int64>(key));
                            break;
                        case (DataType.INT64, DataEncoding.OffsetBinary):
                            list.AddRange(pkt.Get<UInt64>(key));
                            break;
                        case (DataType.INT32, DataEncoding.TwosComplement):
                            list.AddRange(pkt.Get<Int32>(key));
                            break;
                        case (DataType.INT32, DataEncoding.OffsetBinary):
                            list.AddRange(pkt.Get<UInt32>(key));
                            break;
                        case (DataType.INT16, DataEncoding.TwosComplement):
                            list.AddRange(pkt.Get<Int16>(key));
                            break;
                        case (DataType.INT16, DataEncoding.OffsetBinary):
                            list.AddRange(pkt.Get<UInt16>(key));
                            break;
                        case (DataType.INT8, DataEncoding.TwosComplement):
                            list.AddRange(pkt.Get<sbyte>(key));
                            break;
                        case (DataType.INT8, DataEncoding.OffsetBinary):
                            list.AddRange(pkt.Get<byte>(key));
                            break;
                        case (DataType.FP64, _):
                            list.AddRange(pkt.Get<double>(key));
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
