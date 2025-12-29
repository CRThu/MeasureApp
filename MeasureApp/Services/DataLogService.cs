using CarrotLink.Core.Logging;
using CarrotLink.Core.Protocols.Models;
using CommunityToolkit.HighPerformance.Buffers;
using CommunityToolkit.Mvvm.ComponentModel;
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
    public partial class DataLogService : ObservableObject, IPacketLogger, IDisposable
    {
        private readonly ConcurrentDictionary<string, DataLogList> logs = new ConcurrentDictionary<string, DataLogList>();
        private bool _disposed = false;

        private readonly object _keyLock = new object();

        public IEnumerable<string> Keys => logs.Keys;

        /// <summary>
        /// Key 列表发生变更的事件
        /// </summary>
        public event Action? KeysChanged;

        public bool Contains(string key) => logs.ContainsKey(key);

        public DataLogList this[string key] => GetOrAddKey(key);

        public DataLogList GetOrAddKey(string key)
        {
            if (logs.TryGetValue(key, out var v1))
                return v1;

            lock (_keyLock)
            {
                if (logs.TryGetValue(key, out var v2))
                    return v2;

                var newList = new DataLogList();
                if (logs.TryAdd(key, newList))
                {
                    KeysChanged?.Invoke();
                    OnPropertyChanged(nameof(Keys));
                }
                return newList;
            }
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
            lock (_keyLock)
            {
                if (logs.TryRemove(key, out var l))
                {
                    l.Dispose();
                    KeysChanged?.Invoke();
                    OnPropertyChanged(nameof(Keys));
                }
            }
        }

        public void Clear()
        {
            lock (_keyLock)
            {
                foreach (var log in logs.Values)
                    log.Dispose();
                logs.Clear();
                KeysChanged?.Invoke();
                OnPropertyChanged(nameof(Keys));
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

        public void Add<T>(string key, T value) where T : unmanaged
        {
            GetOrAddKey(key).Add<T>(value);
        }

        public void AddRange<T>(string key, IEnumerable<T> value) where T : unmanaged
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
                    var list = GetOrAddKey(StringPool.Shared.GetOrAdd($"{from}->{to}.{key}"));

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
