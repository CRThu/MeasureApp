﻿using MeasureApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MeasureApp.Model
{
    public class DataStorage : NotificationObjectBase
    {
        private Dictionary<string, object> lockers = new();
        private ObservableDictionary<string, ObservableCollection<StringDataClass>> _dataStorageDictionary = new();
        public ObservableDictionary<string, ObservableCollection<StringDataClass>> DataStorageDictionary
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

        public void AddKey(string key)
        {
            DataStorageDictionary.Add(key, new ObservableCollection<StringDataClass>());

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

        public IEnumerable<string> GetAllData(string key)
        {
            return DataStorageDictionary[key].Select(str => str.StringData);
        }

        public void ClearAllData(string key)
        {
            lock (lockers[key])
            {
                DataStorageDictionary[key].Clear();
            }
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
            File.WriteAllLines(fileName, GetAllData(key));
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