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
        // sender为key
        public event EventHandler OnDataChanged;

        private Dictionary<string, DataStorageElement> data;
        private Dictionary<string, DataStorageElement> Data
        {
            get => data;
            set
            {
                data = value;
            }
        }

        public IEnumerable<double> this[string key]
        {
            get
            {
                return GetValues(key);
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
                SelectedData.OnDataChanged -= (_, _) => RaisePropertyChanged(() => SelectedData);

                selectedKey = value ?? Keys.FirstOrDefault();

                SelectedData.OnDataChanged += (_, _) => RaisePropertyChanged(() => SelectedData);
                RaisePropertyChanged(() => SelectedData);
            }
        }

        public DataStorageElement SelectedData
        {
            get
            {
                if (selectedKey != null)
                    return Data[SelectedKey];
                return new();
            }
        }

        public DataStorage()
        {
            InitEvent();
            Data = new();
            AddKey("DefaultKey");
            SelectedKey = "DefaultKey";
        }

        public DataStorage(Dictionary<string, DataStorageElement> data) : this()
        {
            foreach (var item in data)
                AddValues(item.Key, item.Value.Y);
        }

        private void InitEvent()
        {
            OnKeysChanged += (_, _) => RaisePropertyChanged(() => Keys);
        }

        // 字典操作方法
        public void AddKey(string key)
        {
            if (!Data.ContainsKey(key))
            {
                Data.Add(key, new DataStorageElement());
                OnKeysChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void RemoveKey(string key)
        {
            if (Data.ContainsKey(key))
            {
                if (key != "DefaultKey")
                    Data.Remove(key);
                OnKeysChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool ContainsKey(string key)
        {
            return Data.ContainsKey(key);
        }

        // 数据操作方法
        public void AddValue(string key, double value)
        {
            if (!Data.ContainsKey(key))
                AddKey(key);
            Data[key].AddY(value);
            OnDataChanged?.Invoke(key, EventArgs.Empty);
        }

        public void AddValue<T>(string key, T value)
        {
            AddValue(key, (decimal)Convert.ChangeType(value, typeof(decimal)));
        }

        public void AddValues(string key, IEnumerable<double> values)
        {
            if (!Data.ContainsKey(key))
                AddKey(key);
            Data[key].AddY(values);
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

        public void SetValues(string key, IEnumerable<double> values)
        {
            if (!Data.ContainsKey(key))
                AddKey(key);
            Data[key].Clear();
            Data[key].AddY(values);
            OnDataChanged?.Invoke(key, EventArgs.Empty);
        }

        public IEnumerable<double> GetValues(string key)
        {
            if (key != null && Data.ContainsKey(key))
                return Data[key].Y;
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
            var data = JsonConvert.DeserializeObject<Dictionary<string, DataStorageElement>>(json, new JsonSerializerSettings() { FloatParseHandling = FloatParseHandling.Decimal });
            return new(data);
        }
    }
}
