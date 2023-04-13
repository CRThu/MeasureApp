using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MeasureApp.Model.Common
{
    /// <summary>
    /// Newtonsoft.Json序列化与反序列化通用配置静态类
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// Newtonsoft.Json配置项
        /// </summary>
        private static readonly JsonSerializerSettings JsonOptions = new()
        {
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter> {
                    new StringEnumConverter()
            }
        };

        /// <summary>
        /// Json序列化方法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">Object</param>
        /// <returns>返回序列化字符串</returns>
        public static string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value, JsonOptions);
        }

        /// <summary>
        /// Json序列化到文件方法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value"></param>
        /// <param name="filepath">json文件路径</param>
        public static void SerializeToFile<T>(T value, string filepath)
        {
            File.WriteAllText(filepath, Serialize(value));
        }

        /// <summary>
        /// Json反序列化方法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="json">序列化字符串</param>
        /// <returns>Object</returns>
        public static T DeSerialize<T>(string json)
        {
            T? jsonObject = JsonConvert.DeserializeObject<T>(json, JsonOptions);
            return jsonObject!;
        }

        /// <summary>
        /// Json从文件反序列化方法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="filepath">json文件路径</param>
        /// <returns>Object</returns>
        public static T DeSerializeFromFile<T>(string filepath)
        {
            string jsonString = File.ReadAllText(filepath);
            return DeSerialize<T>(jsonString);
        }
    }
}
