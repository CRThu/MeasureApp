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
        public static JsonNode? Parse(string json, bool ignoreDoubleQuotes = false)
        {
            return JsonNode.Parse(json);
        }
    }
}
