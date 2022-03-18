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
        public event EventHandler OnDataChangedEvent;

        private ObservableDictionary<string, ObservableRangeCollection<ObservableValue>> dict = new();
        public ObservableDictionary<string, ObservableRangeCollection<ObservableValue>> Dict
        {
            get => dict;
            set
            {
                dict = value;
                RaisePropertyChanged(() => Dict);
                RaisePropertyChanged(() => Keys);
            }
        }

        public dynamic[] this[string key]
        {
            get
            {
                return GetDataCollection(key).ToArray();
            }
        }

        public int Count => Dict.Count;
        public string[] Keys
        {
            get
            {
                return Dict.Keys.ToArray();
            }
        }

        public dynamic[] SelectedData
        {
            get
            {
                if (Dict.ContainsKey(SelectedKey))
                    return GetDataCollection(SelectedKey).ToArray();
                else
                    return null;
            }
        }

        private string selectedKey;
        public string SelectedKey
        {
            get => selectedKey;
            set
            {
                selectedKey = value;
                RaisePropertyChanged(() => SelectedKey);
            }
        }

        public DataStorage()
        {
        }

        public void Load(DataStorage dataStorage)
        {
            Clear();
            foreach (string key in dataStorage.Dict.Keys)
            {
                AddDataCollection(key, dataStorage.GetDataCollection(key));
            }
        }

        private void Clear()
        {
            Dict.Clear();
        }

        public void AddKey(string key)
        {
            // async
            Application.Current.Dispatcher.Invoke(() =>
            {
                object locker = new();
                Dict.Add(key, new ObservableRangeCollection<ObservableValue>());
                RaisePropertyChanged(() => Keys);
                BindingOperations.EnableCollectionSynchronization(Dict[key], locker);
            });
        }

        public void RemoveKey(string key)
        {
            Dict.Remove(key);
            RaisePropertyChanged(() => Keys);
        }

        public void AddData(string key, dynamic value)
        {
            if (!Dict.ContainsKey(key))
            {
                AddKey(key);
            }
            Dict[key].Add(new ObservableValue() { Value = value });

            OnDataChangedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void AddDataCollection(string key, IEnumerable<dynamic> values)
        {
            if (!Dict.ContainsKey(key))
            {
                AddKey(key);
            }
            Dict[key].AddRange(values.Select(value => new ObservableValue() { Value = value }));
            OnDataChangedEvent?.Invoke(this, EventArgs.Empty);
        }

        public IEnumerable<dynamic> GetDataCollection(string key)
        {
            return Dict[key].Select(str => str.Value);
        }

        public void ClearDataCollection(string key)
        {
            Dict[key].Clear();
            OnDataChangedEvent?.Invoke(this, EventArgs.Empty);
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
            foreach (string key in Dict.Keys)
            {
                Save(key, GenerateFileName(key, extension, titleName, isAddDateTime));
            }
        }
    }
}
