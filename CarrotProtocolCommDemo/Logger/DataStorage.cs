using CarrotProtocolLib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace CarrotProtocolCommDemo.Logger
{
    public partial class DataStorage<T> : ObservableObject
    {
        private Dictionary<string, List<T>> StorageDict;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayData))]
        private string currentKey;

        public List<T> DisplayData
        {
            get
            {
                lock (LockObject)
                {
                    if (CurrentKey is not null && StorageDict.TryGetValue(CurrentKey, out var value))
                    {
                        return value;
                    }
                    else
                    {
                        return new List<T>();
                    }
                }
            }
        }

        public string[] DisplayKeys
        {
            get
            {
                lock (LockObject)
                {
                    if (StorageDict is not null)
                    {
                        return StorageDict.Keys.ToArray();
                    }
                    else
                    {
                        return Array.Empty<string>();
                    }
                }
            }
        }

        /// <summary>
        /// 线程安全管理
        /// </summary>
        private readonly object LockObject;

        public DataStorage()
        {
            StorageDict = new();
            LockObject = new object();
        }

        public void CreateKeyIfNotExist(string key)
        {
            lock (LockObject)
            {
                if (!StorageDict.ContainsKey(key))
                {
                    var list = new List<T>();
                    StorageDict.Add(key, list);
                }
            }

            OnPropertyChanged(nameof(DisplayKeys));
        }

        public void AddValue(string key, T value)
        {
            CreateKeyIfNotExist(key);

            lock (LockObject)
            {
                StorageDict[key].Add(value);
            }

            OnPropertyChanged(nameof(DisplayData));
        }

        public void AddValue(string key, IEnumerable<T> value)
        {
            CreateKeyIfNotExist(key);

            lock (LockObject)
            {
                StorageDict[key].AddRange(value);
            }

            OnPropertyChanged(nameof(DisplayData));
        }

        public void RemoveKey(string key)
        {
            lock (LockObject)
            {
                if (StorageDict.ContainsKey(key))
                {
                    StorageDict.Remove(key);
                }
            }

            OnPropertyChanged(nameof(DisplayKeys));
            OnPropertyChanged(nameof(DisplayData));
        }

        public void RemoveValues(string key)
        {
            lock (LockObject)
            {
                StorageDict[key].Clear();
            }

            OnPropertyChanged(nameof(DisplayData));
        }
    }
}
