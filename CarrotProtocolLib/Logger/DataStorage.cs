using CarrotProtocolLib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Logger
{
    public partial class DataStorage<T> : ObservableObject
    {
        [ObservableProperty]
        private ObservableDictionary<string, ObservableCollection<T>> storageDict;

        /// <summary>
        /// 线程安全管理
        /// </summary>
        private object LockObject;

        public DataStorage()
        {
            storageDict = new();
            LockObject = new object();
        }

        public void AddKey(string key)
        {
            if (!StorageDict.ContainsKey(key))
                StorageDict.Add(key, new ObservableCollection<T>());
        }

        public void AddValue(string key, T value)
        {
            lock (LockObject)
            {
                AddKey(key);
                StorageDict[key].Add(value);
            }
        }

        //public void AddValue(string key, IEnumerable<T> value)
        //{
        //    lock (LockObject)
        //    {
        //        AddKey(key);
        //        StorageDict[key].AddRange(value);
        //    }
        //}

        public void RemoveKey(string key)
        {
            lock (LockObject)
            {
                StorageDict.Remove(key);
            }
        }

        public void RemoveValues(string key)
        {
            lock (LockObject)
            {
                StorageDict[key].Clear();
            }
        }
    }
}
