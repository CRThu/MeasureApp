using CommunityToolkit.Mvvm.ComponentModel;
using MeasureApp.Model.Log;
using MeasureApp.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;

namespace MeasureApp.Services
{
    public partial class DataLogList : ObservableObject
    {
        private readonly Channel<DataLogValue> _channel;

        private List<DataLogValue> _items = new List<DataLogValue>();
        private readonly object _itemsLock = new object();

        public List<DataLogValue> Items => _items;
        public event Action? DataAdded;

        public DataLogValue this[int index]
        {
            get
            {
                lock (_itemsLock)
                    return _items[index];
            }
        }

        private int _count;
        public int Count => _count;

        private DataLogValue _latestValue;
        public DataLogValue LatestValue => _latestValue;

        public string Summary
        {
            get
            {
                var val = LatestValue;
                int len = Count;
                if (LatestValue.Type == DataLogValue.ValueType.Null)
                    return "[NULL](0)";
                return $"[{val.Type}]({len})";
            }
        }

        // 消费者任务
        private readonly Task _consumerTask;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private bool _disposed = false;

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

            BindingOperations.EnableCollectionSynchronization(_items, _itemsLock);
        }

        private async Task ProcessQueueAsync(CancellationToken token)
        {
            var buffer = new List<DataLogValue>();
            var reader = _channel.Reader;

            try
            {
                while (!token.IsCancellationRequested && await reader.WaitToReadAsync(token))
                {
                    while (reader.TryRead(out var item))
                        buffer.Add(item);

                    if (buffer.Count > 0)
                    {
                        lock (_itemsLock)
                        {
                            _items.AddRange(buffer);
                            _count = _items.Count;
                            _latestValue = _items.Last();
                            buffer.Clear();
                        }
                        DataAdded?.Invoke();


                        await App.Current.Dispatcher.BeginInvoke(() =>
                        {
                            lock (_itemsLock)
                            {
                                CollectionViewSource.GetDefaultView(Items).Refresh();
                            }
                        }, DispatcherPriority.Background);
                        OnPropertyChanged(nameof(Count));
                        OnPropertyChanged(nameof(LatestValue));
                        OnPropertyChanged(nameof(Summary));
                    }

                    await Task.Delay(50, token);
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

        public void AddRange<T>(IEnumerable<T> values) where T : unmanaged
        {
            foreach (var v in values)
            {
                _channel.Writer.TryWrite(DataLogValue.From(v));
            }
        }

        public void AddRange<T>(ReadOnlySpan<T> values) where T : unmanaged
        {
            foreach (var v in values)
            {
                _channel.Writer.TryWrite(DataLogValue.From(v));
            }
        }

        public DataLogValue[] GetSnapshot()
        {
            lock (_itemsLock)
            {
                return _items.ToArray();
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _channel.Writer.TryComplete();
            _cts.Dispose();
        }
    }
}
