using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.Script
{
    /// <summary>
    /// Manages the script's execution environment variables in a type-safe and thread-safe manner.
    /// </summary>
    public class ScriptEnvironment : StringParameterCollection
    {
        private readonly object _lock = new();

        public ScriptEnvironment() : base(new Dictionary<string, string>())
        {
        }

        /// <summary>
        /// Sets or updates an environment variable. This operation is thread-safe.
        /// </summary>
        /// <param name="key">The key of the environment variable.</param>
        /// <param name="value">The value of the environment variable.</param>
        public void Set(string key, string value)
        {
            lock (_lock)
            {
                _parameters[key] = value;
            }
        }

        /// <summary>
        /// Clears all environment variables. This operation is thread-safe.
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _parameters.Clear();
            }
        }


        /// <summary>
        /// Gets a snapshot of all variables, converting them to decimal for expression evaluation.
        /// </summary>
        public Dictionary<string, decimal> GetAllVariables()
        {
            lock (_lock)
            {
                return _parameters
                    .Where(kvp => !kvp.Value.Contains('[') && decimal.TryParse(kvp.Value, out _))
                    .ToDictionary(kvp => kvp.Key, kvp => Convert.ToDecimal(kvp.Value));
            }
        }
    }
}
