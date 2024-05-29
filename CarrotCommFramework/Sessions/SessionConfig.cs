using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Sessions
{
    public enum ComponentType
    {
        SESSION,
        STREAM,
        PROTOCOL,
        LOGGER,
        SERVICE
    }

    public class SessionComponentInfo
    {
        public ComponentType Type { get; set; }
        public string ServiceName { get; set; }
        public string InstanceName { get; set; }
        public string[] Params { get; set; } = [];
    }

    // TODO
    public class SessionConfig
    {
        public static SessionConfig Default { get; set; } = new()
        {
            PresetSessionCommands = ["DEFAULT_SESSION"],
            PresetProtocolCommands = ["RAPV1://RAPV1"],
            PresetLoggerCommands = ["CONSOLE://CON1", "NLOG://NLOG1"],
            PresetServiceCommands = ["RECV", "PARSE"]
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
    }
}
