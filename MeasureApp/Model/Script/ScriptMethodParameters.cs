using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.Script
{
    public class ScriptMethodParameters
    {
        private Dictionary<string, string> _kvp = new Dictionary<string, string>();
        private IDictionary<string, string> _defaults;

        public ScriptMethodParameters(IDictionary<string, string> kvps)
        {
            foreach (var kv in kvps)
            {
                _kvp.Add(kv.Key, kv.Value);
            }
        }

        public void SetDefaults(IDictionary<string, string> defaults)
        {
            _defaults = defaults;
        }

        public T Get<T>(string key)
        {
            string val;
            if (_kvp.TryGetValue(key, out val) || _defaults.TryGetValue(key, out val))
            {
                return (T)Convert.ChangeType(val, typeof(T));
            }
            else
                return default;
        }


    }
}
