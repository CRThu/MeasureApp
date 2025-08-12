using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.Script
{
    /// <summary>
    /// A base class for handling collections of string-based key-value parameters.
    /// Provides powerful, type-safe retrieval and conversion capabilities.
    /// </summary>
    public abstract class StringParameterCollection
    {
        protected readonly IDictionary<string, string> _parameters;

        protected StringParameterCollection(IDictionary<string, string> parameters)
        {
            // Use a case-insensitive dictionary for flexibility in script writing (e.g., "key" vs "Key")
            _parameters = new Dictionary<string, string>(parameters, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the value associated with the specified key, converting it to the requested type T.
        /// </summary>
        /// <typeparam name="T">The target type to convert the value to (e.g., int, bool, double, string).</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="defaultValue">The value to return if the key is not found.</param>
        /// <returns>The converted value, or the default value if the key is not found.</returns>
        /// <exception cref="FormatException">Thrown when the value cannot be converted to the target type.</exception>
        public T Get<T>(string key, T defaultValue = default)
        {
            if (_parameters.TryGetValue(key, out var valueString))
            {
                try
                {
                    // Handle common types directly for better performance and control.
                    var targetType = typeof(T);
                    if (targetType == typeof(string))
                    {
                        return (T)(object)valueString;
                    }
                    // Use InvariantCulture to ensure consistent parsing regardless of system locale (e.g., for decimal points).
                    return (T)Convert.ChangeType(valueString, targetType, CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    throw new FormatException($"Failed to convert parameter '{key}' with value '{valueString}' to type '{typeof(T).Name}'.", ex);
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Tries to get the value associated with the specified key and convert it to the requested type T.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the converted value if the key was found and conversion succeeded; otherwise, the default value for the type T.</param>
        /// <returns>true if the key was found and the value was successfully converted; otherwise, false.</returns>
        public bool TryGet<T>(string key, out T value)
        {
            try
            {
                value = Get<T>(key, default);
                // Check if the key actually existed, as Get might return default(T)
                return _parameters.ContainsKey(key);
            }
            catch
            {
                value = default;
                return false;
            }
        }
    }
}
