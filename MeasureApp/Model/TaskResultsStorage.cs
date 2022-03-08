using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model
{
    public class TaskResultsStorage
    {
        public int[] Version { get; set; }
        public DateTime LastModifyTime { get; set; }
        public Dictionary<string, Dictionary<string, string>> Value { get; set; }

        public TaskResultsStorage()
        {
            Version = new[] { 1, 0 };
            LastModifyTime = DateTime.Now;
            Value = new();
        }

        public void Set(string resultId, string key, string value)
        {
            LastModifyTime = DateTime.Now;
            if (!Value.ContainsKey(resultId))
                Value.Add(resultId, new());
            if (!Value[resultId].ContainsKey(key))
                Value[resultId].Add(key, value);
            else
                Value[resultId][key] = value;
        }

        public string Get(string resultId, string key)
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
    }
}
