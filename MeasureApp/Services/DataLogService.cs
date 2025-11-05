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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MeasureApp.Services
{

    public partial class DataLogList : ObservableObject, IDisposable
    {
        // Use BlockingCollection as a thread-safe producer-consumer queue.
        // 使用 BlockingCollection 作为线程安全的生产者-消费者队列。
        private readonly BlockingCollection<IEnumerable<DataLogValue>> _queue = new BlockingCollection<IEnumerable<DataLogValue>>(new ConcurrentQueue<IEnumerable<DataLogValue>>());

        // The internal, mutable list. Access MUST be synchronized.
        // 内部的可变列表。访问必须同步。
        private readonly List<DataLogValue> _items = new List<DataLogValue>();

        // A lock object dedicated to protecting the _items list.
        // 一个专用于保护 _items 列表的锁对象。
        private readonly object _itemsLock = new object();

        // Cancellation token to gracefully shut down the background processing task.
        // 用于优雅地关闭后台处理任务的取消令牌。
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly Task _consumerTask;
        private bool _disposed = false;

        // Public property returns a read-only wrapper for safe data binding.
        // 公共属性返回一个只读包装器，用于安全的数据绑定。
        public IReadOnlyList<DataLogValue> Items => _items.AsReadOnly();

        public DataLogList()
        {
            // Start the long-running consumer task in the background.
            // 在后台启动长期运行的消费者任务。
            _consumerTask = Task.Run(() => ProcessQueue(_cts.Token));
        }

        /// <summary>
        /// The main loop for the consumer task. It waits for data and processes it in batches.
        /// 消费者任务的主循环。它会等待数据并分批处理。
        /// </summary>
        private async Task ProcessQueue(CancellationToken token)
        {
            try
            {
                // This loop will run until cancellation is requested.
                while (!token.IsCancellationRequested)
                {
                    // Block and wait for the first item to arrive. This is very CPU efficient.
                    // 阻塞等待第一个项目的到达。这是非常高效的CPU使用方式。
                    var batch = _queue.Take(token);
                    var batchAsList = batch as List<DataLogValue> ?? batch.ToList();
                    if (batchAsList.Count == 0)
                        continue;

                    // If we have items, update the collection and notify the UI.
                    lock (_itemsLock)
                    {
                        _items.EnsureCapacity(_items.Count + batchAsList.Count);
                        _items.AddRange(batchAsList);
                    }

                    // Notify UI on the correct thread.
                    // 在正确的线程上通知UI。
                    _ = Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        OnPropertyChanged(nameof(Items));
                    });

                    // Optional: Add a small delay to control the maximum UI update frequency.
                    // For example, await Task.Delay(50, token); to update at most 20 times per second.
                    // If you remove this, UI will be updated as fast as batches are formed.
                    // 可选：添加一个小的延迟来控制UI更新的最大频率。
                    // 例如 `await Task.Delay(50, token);` 来实现最多每秒更新20次。
                    // 如果移除这行，UI会以批次形成的最快速度更新。
                    await Task.Delay(50, token);
                }
            }
            catch (OperationCanceledException)
            {
                // This is expected when Dispose is called. Gracefully exit the loop.
                // 当 Dispose 被调用时，这是预期的异常。优雅地退出循环。
            }
            catch (Exception ex)
            {
                // Log any unexpected errors from the background task.
                // 记录后台任务的任何意外错误。
                Debug.WriteLine($"Error in DataLogList consumer task: {ex}");
            }
        }

        /// <summary>
        /// Provides a thread-safe snapshot of the current data.
        /// This is the correct way to access the collection for enumeration from a background thread.
        /// 提供当前数据的线程安全快照。
        /// 这是从后台线程访问集合以进行枚举的正确方法。
        /// </summary>
        public DataLogValue[] GetSnapshot()
        {
            lock (_itemsLock)
            {
                return _items.ToArray();
            }
        }

        // The Add methods are now the "Producers". They just add to the queue and return immediately.
        // Add 方法现在是“生产者”。它们只负责将数据添加到队列中并立即返回。
        public void Add<T>(T value)
        {
            _queue.Add(new DataLogValue[] { DataLogValue.From<T>(value) });
        }

        public void AddRange<T>(IEnumerable<T> values)
        {
            var list = values.Select(DataLogValue.From<T>).ToList();
            if (list.Count > 0)
                _queue.Add(list);
        }

        public void AddRange<T>(ReadOnlySpan<T> values)
        {
            if (values.IsEmpty)
                return;

            var list = new List<DataLogValue>(values.Length);
            foreach (var value in values)
            {
                list.Add(DataLogValue.From<T>(value));
            }
            _queue.Add(list);
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
                // Signal the background task to stop.
                // 通知后台任务停止。
                _cts.Cancel();

                // Wait for the task to complete its final processing.
                // A timeout is good practice to prevent deadlocks.
                // 等待任务完成其最终处理。设置超时是防止死锁的好习惯。
                try
                {
                    _consumerTask.Wait(500);
                }
                catch (AggregateException) { /* Can be ignored here */ }

                // Dispose managed resources.
                // 释放托管资源。
                _cts.Dispose();
                _queue.Dispose();
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


        public bool TryGetValue(string key, out DataLogList dataLogList)
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
        /// Copies the data for a given key to the system clipboard.
        /// Each data point is on a new line, suitable for pasting into Excel.
        /// 将指定key的数据复制到系统剪贴板。
        /// 每个数据点占一行，适合粘贴到Excel。
        /// </summary>
        /// <param name="key">The key of the data log to copy.</param>
        public void CopyToClipboard(string key)
        {
            if (string.IsNullOrEmpty(key) || !logs.TryGetValue(key, out var dataLogList))
            {
                // Optionally, provide feedback to the user that the key was not found.
                // （可选）可以向用户提供反馈，告知未找到该key。
                return;
            }

            // Get a thread-safe snapshot of the data to avoid locking during string building.
            // 获取数据的线程安全快照，以避免在构建字符串时锁定集合。
            var dataSnapshot = dataLogList.GetSnapshot();

            if (dataSnapshot.Length == 0)
            {
                return; // Nothing to copy.
            }

            // Use StringBuilder for efficient string concatenation.
            // 使用 StringBuilder 高效拼接字符串。
            var sb = new StringBuilder();
            foreach (var value in dataSnapshot)
            {
                sb.AppendLine(value.GetValueString());
            }

            // Clipboard operations must be performed on the UI thread.
            // 剪贴板操作必须在UI线程上执行。
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    Clipboard.SetText(sb.ToString());
                    // Optionally, show a success message.
                    // (可选) 显示成功信息。
                }
                catch (Exception ex)
                {
                    // Handle potential exceptions from clipboard access.
                    // 处理访问剪贴板时可能发生的异常。
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
