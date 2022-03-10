using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.DynamicCompilation
{
    public class TaskResultsStorage
    {
        [JsonIgnore]
        public static int[] defaultClassVersion = new[] { 1, 0 };

        public int[] ClassVersion { get; set; }
        public int[] AssemblyVersion { get; set; }
        public DateTime LastModifyTime { get; set; }

        public bool IsAutoLoadAssemblyDll { get; set; }
        public string AssemblyDllPath { get; set; }
        public Dictionary<string, Dictionary<string, RunTaskItemSaveData>> Value { get; set; }

        public TaskResultsStorage()
        {
            ClassVersion = defaultClassVersion;
            AssemblyVersion = GetAssemblyVersionArray();
            LastModifyTime = DateTime.Now;
            IsAutoLoadAssemblyDll = true;
            Value = new();
        }

        public void Set(string resultId, string key, (string pv, string rv) value)
        {
            Set(resultId, key, new RunTaskItemSaveData(value));
        }

        public void Set(string resultId, string key, RunTaskItemSaveData value)
        {
            LastModifyTime = DateTime.Now;
            if (!Value.ContainsKey(resultId))
                Value.Add(resultId, new());
            if (!Value[resultId].ContainsKey(key))
                Value[resultId].Add(key, value);
            else
                Value[resultId][key] = value;
        }

        public RunTaskItemSaveData Get(string resultId, string key)
        {
            return Value[resultId][key];
        }

        public static TaskResultsStorage Deserialize(string taskResultsStorage)
        {
            return JsonConvert.DeserializeObject<TaskResultsStorage>(taskResultsStorage, new JsonSerializerSettings() { FloatParseHandling = FloatParseHandling.Decimal });
        }

        public static string Serialize(TaskResultsStorage taskResultsStorage)
        {
            return JsonConvert.SerializeObject(taskResultsStorage, new JsonSerializerSettings() { FloatParseHandling = FloatParseHandling.Decimal });
        }

        public static int[] GetAssemblyVersionArray()
        {
            return Assembly.GetEntryAssembly().GetName().Version.ToString().Split('.').Select(s => Convert.ToInt32(s)).ToArray();
        }

        public bool IsEqualCurrentVersion()
        {
            if (string.Join('.', ClassVersion) != string.Join('.', defaultClassVersion))
                return false;
            if (string.Join('.', GetAssemblyVersionArray()) != string.Join('.', AssemblyVersion))
                return false;
            return true;
        }
    }
}
