using DryIoc.ImTools;
using NationalInstruments.Restricted;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CarrotCommFramework.Sessions.SessionComponentInfo;

namespace CarrotCommFramework.Sessions
{

    public class SessionComponentInfo
    {
        public enum ComponentType
        {
            SESSION,
            STREAM,
            PROTOCOL,
            LOGGER,
            SERVICE
        }

        public static Dictionary<string, ComponentType> ComponentDict = new()
        {
            { "session", ComponentType.SESSION },
            { "stream", ComponentType.STREAM },
            { "protocol", ComponentType.PROTOCOL },
            { "logger", ComponentType.LOGGER },
            { "service", ComponentType.SERVICE }
        };

        public const string ComponentServicePropertyName = "service";
        public const string ComponentInstancePropertyName = "instance";

        public ComponentType Type { get; set; }
        public string ServiceName { get; set; }
        public string InstanceName { get; set; }
        public Dictionary<string, string> Params { get; set; } = [];
    }

    public static class SessionConfigInstanceCounter
    {
        public static readonly Dictionary<(ComponentType, string), int> AutoGenerateInstanceCount = new();
    }

    // TODO
    public class SessionConfig
    {
        public static SessionConfig Default { get; } = new();


        //PresetSessionCommands = ["DEFAULT_SESSION"],
        //PresetProtocolCommands = ["RAPV1://RAPV1"],
        //PresetLoggerCommands = ["CONSOLE://CON1", "NLOG://NLOG1"],
        //PresetServiceCommands = ["RECV", "PARSE"]

        public static SessionConfig Empty { get; } = new();

        public List<SessionComponentInfo> Components { get; private set; } = [];

        public SessionConfig()
        {

        }

        public SessionConfig(SessionConfig config)
        {
            Components = new List<SessionComponentInfo>(config.Components);
        }

        public static SessionConfig Create(string componentsConfig)
        {
            SessionConfig sc = SessionConfigParser.Parse(componentsConfig);
            return sc;
        }

        public void GenerateInstanceName(SessionComponentInfo info)
        {
            if (string.IsNullOrEmpty(info.InstanceName))
            {
                int newVal = 0;
                if (SessionConfigInstanceCounter.AutoGenerateInstanceCount.TryGetValue((info.Type, info.ServiceName), out int val))
                {
                    SessionConfigInstanceCounter.AutoGenerateInstanceCount[(info.Type, info.ServiceName)] = (val + 1);
                    newVal = (val + 1);
                }
                else
                {
                    SessionConfigInstanceCounter.AutoGenerateInstanceCount.Add((info.Type, info.ServiceName), 0);
                }
                info.InstanceName = $"{info.ServiceName}_inst{newVal}";
                Console.WriteLine($"({info.Type}, {info.ServiceName}):{newVal}");
            }
        }

        public void Add(SessionComponentInfo info)
        {
            GenerateInstanceName(info);
            Components.Add(info);
        }
    }
}
