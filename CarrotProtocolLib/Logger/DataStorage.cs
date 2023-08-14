using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Logger
{
    public class DataStorage<T> where T : struct
    {
        public Dictionary<string, List<T>> StorageDict { get; set; }

        /// <summary>
        /// 线程安全管理
        /// </summary>
        private object LockObject;

        public DataStorage()
        {
            StorageDict = new();
        }

        private void CreateKeyIfNotExist(string key)
        {
            if (!StorageDict.ContainsKey(key))
                StorageDict.Add(key, new List<T>());
        }

        public void AddValue(string key, T value)
        {
            lock (LockObject)
            {
                CreateKeyIfNotExist(key);
                StorageDict[key].Add(value);
            }
        }

        public void AddValue(string key, IEnumerable<T> value)
        {
            lock (LockObject)
            {
                CreateKeyIfNotExist(key);
                StorageDict[key].AddRange(value);
            }
        }

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
