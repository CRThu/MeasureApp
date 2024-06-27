using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CarrotCommFramework.Util
{
    public class JsonParser
    {
        public static JsonNode? ParseToJsonNode(string json, bool ignoreDoubleQuotes = false)
        {
            return JsonNode.Parse(json);
        }

        public static JsonDocument? ParseToJsonDocument(string json, bool ignoreDoubleQuotes = false)
        {
            return JsonDocument.Parse(json);
        }
    }
}
