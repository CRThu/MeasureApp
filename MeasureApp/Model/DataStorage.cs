using MeasureApp.Model.Common;
using MeasureApp.ViewModel;
using Newtonsoft.Json;
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
        public event EventHandler OnKeysChanged;
        public event EventHandler OnSelectedKeyChanged;
        public event EventHandler OnSelectedDataChanged;
        // sender为key
        public event EventHandler OnDataChanged;

        private Dictionary<string, ObservableRangeCollection<decimal>> data = new();
        private Dictionary<string, ObservableRangeCollection<decimal>> Data
        {
            get => data;
            set
            {
                data = value;
            }
        }

        public decimal[] this[string key]
        {
            get
            {
                return GetValues(key).ToArray();
            }
            set
            {
                SetValues(key, value);
            }
        }

        public string[] Keys
        {
            get
            {
                return Data.Keys.ToArray();
            }
        }

        // 选中的键值与数据
        private string selectedKey;
        public string SelectedKey
        {
            get => selectedKey;
            set
            {
                // 删除选中元素后value传入null
                selectedKey = value ?? Keys.FirstOrDefault();
                OnSelectedKeyChanged?.Invoke(this, EventArgs.Empty);
                OnSelectedDataChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public IEnumerable<decimal> SelectedData
        {
            get
            {
                if (selectedKey != null)
                    return Data[selectedKey];
                return null;
            }
            set
            {
                SetValues(SelectedKey, value);
                OnSelectedDataChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public DataStorage()
        {
            InitEvent();
        }

        public DataStorage(Dictionary<string, IEnumerable<decimal>> data) : this()
        {
            foreach (var item in data)
                AddValues(item.Key, item.Value);
        }

        private void InitEvent()
        {
            OnKeysChanged += (_, _) => RaisePropertyChanged(() => Keys);
            OnSelectedKeyChanged += (_, _) => RaisePropertyChanged(() => SelectedKey);
            OnSelectedDataChanged += (_, _) => RaisePropertyChanged(() => SelectedData);
            OnDataChanged += DataStorage_SelectedKeyOnDataChanged;
        }

        private void DataStorage_SelectedKeyOnDataChanged(object sender, EventArgs e)
        {
            if ((string)sender == selectedKey)
                OnSelectedDataChanged?.Invoke(this, EventArgs.Empty);
        }

        // 字典操作方法
        public void AddKey(string key)
        {
            if (!Data.ContainsKey(key))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    object locker = new();
                    Data.Add(key, new ObservableRangeCollection<decimal>());
                    BindingOperations.EnableCollectionSynchronization(Data[key], locker);
                    RaisePropertyChanged(() => Keys);
                });
                OnKeysChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void RemoveKey(string key)
        {
            if (Data.ContainsKey(key))
            {
                Data.Remove(key);
                OnKeysChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        // 数据操作方法
        public void AddValue(string key, decimal value)
        {
            if (!Data.ContainsKey(key))
                AddKey(key);
            Data[key].Add(value);
            OnDataChanged?.Invoke(key, EventArgs.Empty);
        }

        public void AddValue<T>(string key, T value)
        {
            AddValue(key, (decimal)Convert.ChangeType(value, typeof(decimal)));
        }

        public void AddValues(string key, IEnumerable<decimal> values)
        {
            if (!Data.ContainsKey(key))
                AddKey(key);
            Data[key].AddRange(values);
            OnDataChanged?.Invoke(key, EventArgs.Empty);
        }

        public void AddValues<T>(string key, IEnumerable<T> values)
        {
            AddValues(key, values.Select(v => (decimal)Convert.ChangeType(v, typeof(decimal))));
        }

        public void ClearValues(string key)
        {
            if (!Data.ContainsKey(key))
                AddKey(key);
            Data[key].Clear();
            OnDataChanged?.Invoke(key, EventArgs.Empty);
        }

        public void SetValues(string key, IEnumerable<decimal> values)
        {
            if (!Data.ContainsKey(key))
                AddKey(key);
            Data[key].Clear();
            Data[key].AddRange(values);
            OnDataChanged?.Invoke(key, EventArgs.Empty);
        }

        public IEnumerable<decimal> GetValues(string key)
        {
            if (key != null && Data.ContainsKey(key))
                return Data[key];
            else
                return null;
        }

        public IEnumerable<T> GetValues<T>(string key)
        {
            if (key != null && Data.ContainsKey(key))
                return GetValues(key).Select(v => (T)Convert.ChangeType(v, typeof(T)));
            else
                return null;
        }

        // 文件操作方法
        public static string GenerateDateTimeNow()
        {
            return DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
        }

        public static string GenerateFileName(string key, string extension = "txt", string titleName = "DataStorage", bool isAddDateTime = true)
        {
            return $"{titleName}_{key}_{(isAddDateTime ? GenerateDateTimeNow() : string.Empty)}.{extension}";
        }

        // 序列化与反序列化
        public static string Serialize(DataStorage ds)
        {
            //var options = new JsonSerializerOptions
            //{
            //    IncludeFields = true
            //};
            //string json = System.Text.Json.JsonSerializer.Serialize(DataStorageInstance, options);
            return JsonConvert.SerializeObject(ds.Data, new JsonSerializerSettings() { FloatParseHandling = FloatParseHandling.Decimal });
        }

        public static DataStorage DeSerialize(string json)
        {
            //var options = new JsonSerializerOptions
            //{
            //    IncludeFields = true
            //};
            //DataStorage ds = System.Text.Json.JsonSerializer.Deserialize<DataStorage>(json, options);
            var data = JsonConvert.DeserializeObject<Dictionary<string, IEnumerable<decimal>>>(json, new JsonSerializerSettings() { FloatParseHandling = FloatParseHandling.Decimal });
            return new(data);
        }
    }
}
