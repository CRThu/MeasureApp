using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model
{
    public class ChipsTrimInfo
    {
        public int[] Version { get; set; }
        public DateTime LastModifyTime { get; set; }
        public Dictionary<string, Dictionary<string, decimal?>> Value { get; set; }

        public ChipsTrimInfo()
        {
            Version = new[] { 1, 0 };
            LastModifyTime = DateTime.Now;
            Value = new();
        }

        public void Set(string chipId, string key, decimal? value)
        {
            LastModifyTime = DateTime.Now;
            if (!Value.ContainsKey(chipId))
                Value.Add(chipId, new());
            if (!Value[chipId].ContainsKey(key))
                Value[chipId].Add(key, value);
            else
                Value[chipId][key] = value;
        }

        public decimal? Get(string chipId, string key)
        {
            return Value[chipId][key];
        }
    }
}
