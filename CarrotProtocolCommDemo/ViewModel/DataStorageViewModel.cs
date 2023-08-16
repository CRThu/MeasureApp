using CarrotProtocolLib.Logger;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolCommDemo.ViewModel
{
    public partial class DataStorageViewModel : ObservableObject
    {
        [ObservableProperty]
        DataStorage<double> ds = new();

        [ObservableProperty]
        string? currentKey = null;

        Random random = new Random();

        public DataStorageViewModel()
        {
            Ds.AddValue("111", 111);
            Ds.AddValue("111", 222);
            Ds.AddValue("111", 333);
            CurrentKey = Ds.StorageDict.Keys.FirstOrDefault();
        }

        [RelayCommand]
        private void DataSourceDebug(object param)
        {
            switch ((string)param)
            {
                case "AddKey":
                    string keyName = Guid.NewGuid().ToString()[0..6];
                    Ds.AddKey(keyName);
                    CurrentKey = keyName;
                    break;
                case "RemoveKey":
                    if (CurrentKey is not null && Ds.StorageDict.ContainsKey(CurrentKey))
                        Ds.RemoveKey(CurrentKey);
                    CurrentKey = Ds.StorageDict.Keys.FirstOrDefault();
                    break;
                case "AddValue":
                    if (CurrentKey is not null)
                        ds.AddValue(CurrentKey, random.Next(255));
                    break;
                case "RemoveValues":
                    if (CurrentKey is not null && Ds.StorageDict.TryGetValue(CurrentKey, out ObservableCollection<double> value))
                        value.Clear();
                    break;
                default:
                    return;
            }
        }
    }
}
