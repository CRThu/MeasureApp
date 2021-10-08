using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp
{
    public class DataStorage : NotificationObjectBase
    {
        private Dictionary<string, ObservableCollection<StringDataClass>> _dataStorageDictionary = new();
        public Dictionary<string, ObservableCollection<StringDataClass>> DataStorageDictionary
        {
            get => _dataStorageDictionary;
            set
            {
                _dataStorageDictionary = value;
                RaisePropertyChanged(() => DataStorageDictionary);
            }
        }

        public void AddData(string key, dynamic value)
        {
            if (!DataStorageDictionary.ContainsKey(key))
            {
                DataStorageDictionary.Add(key, new ObservableCollection<StringDataClass>());
            }
            DataStorageDictionary[key].Add(new StringDataClass() { StringData = value.ToString() });
        }


    }
}
