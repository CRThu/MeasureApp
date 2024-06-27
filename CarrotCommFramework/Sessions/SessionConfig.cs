using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    // TODO
    public class SessionConfig
    {
        public static SessionConfig Default { get; set; } = new()
        {
            PresetSessionCommands = ["DEFAULT_SESSION"],
            PresetProtocolCommands = ["RAPV1://RAPV1"],
            PresetLoggerCommands = ["CONSOLE://CON1", "NLOG://NLOG1"],
            //PresetServiceCommands = ["RECV", "PARSE"]
        };

        public static SessionConfig Empty { get; set; } = new();

        public List<string> PresetSessionCommands { get; set; } = [];
        public List<string> PresetStreamCommands { get; set; } = [];
        public List<string> PresetProtocolCommands { get; set; } = [];
        public List<string> PresetLoggerCommands { get; set; } = [];
        public List<string> PresetServiceCommands { get; set; } = [];

        public SessionConfig()
        {
        }
        public SessionConfig(SessionConfig config)
        {
            PresetSessionCommands = new List<string>(config.PresetSessionCommands);
            PresetStreamCommands = new List<string>(config.PresetStreamCommands);
            PresetProtocolCommands = new List<string>(config.PresetProtocolCommands);
            PresetLoggerCommands = new List<string>(config.PresetLoggerCommands);
            PresetServiceCommands = new List<string>(config.PresetServiceCommands);
        }
    }
}
