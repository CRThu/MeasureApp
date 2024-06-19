using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Encodings.Web;

namespace CarrotCommFramework.Util
{
    /// <summary>
    /// 提供 JSON 序列化和反序列化的帮助方法。
    /// </summary>
    public class SerializationHelper
    {
        /// <summary>
        /// 全局 JSON 序列化器选项。
        /// </summary>
        private static readonly JsonSerializerOptions _globalOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter(), new BytesToHexStringConverter() },
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        /// <summary>
        /// 将对象序列化为 JSON 字符串。
        /// </summary>
        /// <typeparam name="T">要序列化的对象的类型。</typeparam>
        /// <param name="obj">要序列化的对象。</param>
        /// <returns>对象的 JSON 字符串表示形式。</returns>
        public static string SerializeToString<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, _globalOptions);
        }

        /// <summary>
        /// 将 JSON 字符串反序列化为对象。
        /// </summary>
        /// <typeparam name="T">要反序列化的对象的类型。</typeparam>
        /// <param name="json">要反序列化的 JSON 字符串。</param>
        /// <returns>反序列化后的对象。</returns>
        public static T? DeserializeFromString<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, _globalOptions);
        }

        /// <summary>
        /// 将对象持久化到 JSON 文件中。
        /// </summary>
        /// <typeparam name="T">要持久化的对象的类型。</typeparam>
        /// <param name="obj">要持久化的对象。</param>
        /// <param name="filePath">要写入 JSON 文件的文件路径。</param>
        public static void SerializeToFile<T>(T obj, string filePath)
        {
            string json = SerializeToString(obj);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// 从 JSON 文件中加载对象。
        /// </summary>
        /// <typeparam name="T">要加载的对象的类型。</typeparam>
        /// <param name="filePath">要读取 JSON 文件的文件路径。</param>
        /// <returns>加载后的对象。</returns>
        public static T? DeserializeFromFile<T>(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return DeserializeFromString<T>(json);
        }
    }

    public class BytesToHexStringConverter : JsonConverter<byte[]>
    {
        public override byte[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.BytesToHexString());
        }
    }
}
