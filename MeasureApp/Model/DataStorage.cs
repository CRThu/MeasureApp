using MeasureApp.Model.Common;
using MeasureApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MeasureApp.Model
{
    public class DataStorage : NotificationObjectBase
    {
        public delegate void OnDataChanged();
        public event OnDataChanged OnDataChangedEvent;

        private ObservableDictionary<string, ObservableRangeCollection<ObservableValue>> dataStorageDictionary = new();
        public ObservableDictionary<string, ObservableRangeCollection<ObservableValue>> DataStorageDictionary
        {
            get => dataStorageDictionary;
            set
            {
                dataStorageDictionary = value;
                RaisePropertyChanged(() => DataStorageDictionary);
            }
        }

        public DataStorage()
        {
        }

        public void Load(DataStorage dataStorage)
        {
            Clear();
            foreach (string key in dataStorage.DataStorageDictionary.Keys)
            {
                AddDataCollection(key, dataStorage.GetDataCollection(key));
            }
        }

        private void Clear()
        {
            DataStorageDictionary.Clear();
        }

        public void AddKey(string key)
        {
            // async
            Application.Current.Dispatcher.Invoke(() =>
            {
                object locker = new();
                DataStorageDictionary.Add(key, new ObservableRangeCollection<ObservableValue>());
                BindingOperations.EnableCollectionSynchronization(DataStorageDictionary[key], locker);
            });
        }

        public void RemoveKey(string key)
        {
            DataStorageDictionary.Remove(key);
        }

        public void AddData(string key, dynamic value)
        {
            if (!DataStorageDictionary.ContainsKey(key))
            {
                AddKey(key);
            }
            if (value is not Array)
            {
                DataStorageDictionary[key].Add(new ObservableValue() { Value = value });
            }
            else
            {
                foreach (object obj in (Array)value)
                {
                    DataStorageDictionary[key].Add(new ObservableValue() { Value = obj });
                }
            }

            OnDataChangedEvent();
        }

        public void AddDataCollection(string key, IEnumerable<dynamic> values)
        {
            if (!DataStorageDictionary.ContainsKey(key))
            {
                AddKey(key);
            }
            DataStorageDictionary[key].AddRange(values.Select(value => new ObservableValue() { Value = value }));
            OnDataChangedEvent();
        }

        public IEnumerable<dynamic> GetDataCollection(string key)
        {
            return DataStorageDictionary[key].Select(str => str.Value);
        }

        public void ClearDataCollection(string key)
        {
            DataStorageDictionary[key].Clear();
            OnDataChangedEvent();
        }

        public static string GenerateDateTimeNow()
        {
            return DateTime.Now.ToString().Replace('/', '-').Replace(':', '-').Replace(' ', '-');
        }

        public static string GenerateFileName(string key, string extension = "txt", string titleName = "DataStorage", bool isAddDateTime = true)
        {
            if (isAddDateTime)
            {
                return $"{titleName}_{key}_{GenerateDateTimeNow()}.{extension}";
            }
            else
            {
                return $"{titleName}_{key}.{extension}";
            }
        }

        public void Save(string key, string fileName)
        {
            File.WriteAllLines(fileName, GetDataCollection(key).ToList().Select<dynamic, string>(v => v.ToString()));
        }

        public void SaveAll(string extension = "txt", string titleName = "DataStorage", bool isAddDateTime = true)
        {
            foreach (string key in DataStorageDictionary.Keys)
            {
                Save(key, GenerateFileName(key, extension, titleName, isAddDateTime));
            }
        }
    }
}
