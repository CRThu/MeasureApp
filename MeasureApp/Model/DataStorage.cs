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
        [JsonIgnore]
        public Dictionary<string, object> lockers = new();

        private ObservableDictionary<string, ObservableRangeCollection<StringDataClass>> _dataStorageDictionary = new();
        public ObservableDictionary<string, ObservableRangeCollection<StringDataClass>> DataStorageDictionary
        {
            get => _dataStorageDictionary;
            set
            {
                _dataStorageDictionary = value;
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

            // async
            Application.Current.Dispatcher.Invoke(() =>
            {
                lockers.Clear();
            });
        }

        public void AddKey(string key)
        {
            DataStorageDictionary.Add(key, new ObservableRangeCollection<StringDataClass>());

            // async
            Application.Current.Dispatcher.Invoke(() =>
            {
                lockers.Add(key, new object());
                BindingOperations.EnableCollectionSynchronization(DataStorageDictionary[key], lockers[key]);
            });
        }

        public void RemoveKey(string key)
        {
            DataStorageDictionary.Remove(key);

            // async
            Application.Current.Dispatcher.Invoke(() =>
            {
                lockers.Remove(key);
            });
        }

        public void AddData(string key, dynamic value)
        {
            if (!DataStorageDictionary.ContainsKey(key))
            {
                AddKey(key);
            }
            DataStorageDictionary[key].Add(new StringDataClass() { StringData = value.ToString() });
        }

        public void AddDataCollection(string key, IEnumerable<dynamic> values)
        {
            if (!DataStorageDictionary.ContainsKey(key))
            {
                AddKey(key);
            }
            DataStorageDictionary[key].AddRange(values.Select(value=> new StringDataClass() { StringData = value.ToString() }));
        }

        public IEnumerable<string> GetDataCollection(string key)
        {
            return DataStorageDictionary[key].Select(str => str.StringData);
        }

        public void ClearDataCollection(string key)
        {
            DataStorageDictionary[key].Clear();
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
            File.WriteAllLines(fileName, GetDataCollection(key));
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
