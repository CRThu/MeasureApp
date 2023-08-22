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

namespace CarrotProtocolCommDemo.Logger
{
    public partial class DataStorage<T> : ObservableObject
    {
        [ObservableProperty]
        private ObservableDictionary<string, ObservableRangeCollection<T>> storageDict;

        /// <summary>
        /// 线程安全管理
        /// </summary>
        private readonly object DictLockObject;

        /// <summary>
        /// 线程安全管理
        /// </summary>
        private readonly object ListLockObject;

        public DataStorage()
        {
            storageDict = new();
            DictLockObject = new object();
            ListLockObject = new object();
            BindingOperations.EnableCollectionSynchronization(storageDict, DictLockObject);
        }

        public void CreateKeyIfNotExist(string key)
        {
            lock (DictLockObject)
            {
                if (!StorageDict.ContainsKey(key))
                {
                    var list = new ObservableRangeCollection<T>();
                    StorageDict.Add(key, list);
                    BindingOperations.EnableCollectionSynchronization(list, ListLockObject);
                }
            }
        }

        public void AddValue(string key, T value)
        {
            CreateKeyIfNotExist(key);
            lock (DictLockObject)
            {
                lock (ListLockObject)
                {
                    StorageDict[key].Add(value);
                }
            }
        }

        public void AddValue(string key, IEnumerable<T> value)
        {
            CreateKeyIfNotExist(key);
            lock (DictLockObject)
            {
                lock (ListLockObject)
                {
                    StorageDict[key].AddRange(value);
                }
            }
        }

        public void RemoveKey(string key)
        {
            lock (DictLockObject)
            {
                if (StorageDict.ContainsKey(key))
                {
                    BindingOperations.DisableCollectionSynchronization(StorageDict[key]);
                    StorageDict.Remove(key);
                }
            }
        }

        public void RemoveValues(string key)
        {
            lock (DictLockObject)
            {
                lock (ListLockObject)
                {
                    StorageDict[key].Clear();
                }
            }
        }
    }
}
