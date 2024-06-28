using CarrotCommFramework.Services;
using CarrotCommFramework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CarrotCommFramework.Sessions
{
    /// <summary>
    /// 配置解析器
    /// </summary>
    public class SessionConfigParser
    {
        /// <summary>
        /// 从字符串命令解析
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static SessionConfig Parse(string command)
        {
            // 范例
            /*
            {
                "session": { "service": "session", "instance" : "session1" },
                "stream": { "service": "com", "instance" : "com250", "baudrate" : "115200", "databits": "8", "parity": "n", "stopbits": "1" },
                "protocol": { "service": "cdpv1" },
                "logger": [ { "service": "consolelogger" }, { "service": "datalogger", "instance": "dl1" } ],
                "service": [ { "service": "recv" }, { "service": "parse" } ]
            }

             */

            var jsonDocument = JsonParser.ParseToJsonDocument(command, true)!;
            var root = jsonDocument.RootElement;

            SessionConfig cfg = new();
            if (root.ValueKind == JsonValueKind.Object)
            {
                // Get Property Name: session stream protocol logger service
                foreach (JsonProperty property in root.EnumerateObject())
                {
                    //Console.WriteLine($"{property.Name}: {property.Value}");
                    if (property.Value.ValueKind == JsonValueKind.Object)
                    {
                        var info = ParseInstance(property, property.Value);
                        cfg.Add(info);
                    }
                    else if (property.Value.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement inst in property.Value.EnumerateArray())
                        {
                            var info = ParseInstance(property, inst);
                            cfg.Add(info);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            return cfg;
        }

        public static SessionComponentInfo ParseInstance(JsonProperty component, JsonElement inst)
        {
            SessionComponentInfo info = new()
            {
                Type = SessionComponentInfo.ComponentDict[component.Name]
            };

            if (inst.ValueKind == JsonValueKind.Object)
            {
                // Get Property Name: service instance etc..
                foreach (JsonProperty p in inst.EnumerateObject())
                {
                    if (p.Value.ValueKind != JsonValueKind.String)
                        throw new NotImplementedException();
                    switch (p.Name)
                    {
                        case SessionComponentInfo.ComponentServicePropertyName:
                            info.ServiceName = p.Value.GetString()!;
                            break;
                        case SessionComponentInfo.ComponentInstancePropertyName:
                            info.InstanceName = p.Value.GetString()!;
                            break;
                        default:
                            info.Params.Add(p.Name, p.Value.GetString()!);
                            break;
                    }
                }
                return info;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
