using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.DynamicCompilation
{
    /// <summary>
    /// 任务运行页函数参数与返回值类型
    /// 支持字符串:
    /// (1,2,3,4,5) or {"parA":1,"parB":2,"parC":3,"parD":4}
    /// 使用Map(string[])定义参数名
    /// </summary>
    public class TaskMethodValue
    {
        public Dictionary<string, double> Values { get; set; } = new();
        public List<string> MapNames { get; set; } = new();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="vals"></param>
        public TaskMethodValue(string vals)
        {
            if (vals.IndexOf(':') < 0)
            {
                string[] valsArray = vals.Replace("(", "").Replace(")", "").Replace(" ", "").Replace(":", "").Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var val in valsArray)
                {
                    Set(Convert.ToDouble(val));
                }
            }
            else
            {
                string[] valsArray = vals.Replace("{", "").Replace("}", "").Replace(" ", "").Split(":,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < valsArray.Length; i += 2)
                {
                    Set(valsArray[i], Convert.ToDouble(valsArray[i + 1]));
                }
            }
        }

        public void Map(string[] names)
        {
            MapNames.AddRange(names);
        }

        public void Set(double value)
        {
            Set((Values.Count).ToString(), value);
        }

        public void Set(string key, double value)
        {
            if (!Values.ContainsKey(key))
                Values.Add(key, value);
            else
                Values[key] = value;
        }

        public double Get(string k)
        {
            return Values[k];
        }

        public double Get(int k)
        {
            if (MapNames.Count > 0)
                return Values[MapNames[k]];
            else
                return Values[k.ToString()];
        }

        public string ValuesToJson()
        {
            return JsonConvert.SerializeObject(Values, new JsonSerializerSettings() { FloatParseHandling = FloatParseHandling.Double });
        }

        public double this[int k] => Get(k);
        public double this[string k] => Get(k);
    }
}
