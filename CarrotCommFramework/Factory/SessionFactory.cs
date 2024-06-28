using CarrotCommFramework.Sessions;
using System.Diagnostics;
using static CarrotCommFramework.Sessions.SessionComponentInfo;

namespace CarrotCommFramework.Factory
{

    public class SessionFactory
    {
        private static readonly SessionFactory current = new();
        public static SessionFactory Current => current;

        public Dictionary<string, Session> Sessions { get; private set; } = new();

        private static Dictionary<ComponentType, Action<string, string, IDictionary<string, string>>> ComponentCreatorDict = new()
        {
            { ComponentType.STREAM, (a, b, c) => StreamFactory.Current.Get(a, b, c) },
            { ComponentType.LOGGER, (a, b, c) => LoggerFactory.Current.Get(a, b, c) },
            { ComponentType.PROTOCOL, (a, b, c) => ProtocolFactory.Current.Get(a, b, c) },
            { ComponentType.SERVICE, (a, b, c) => ServiceFactory.Current.Get(a, b, c) },
        };

        public SessionFactory()
        {
        }

        public Session CreateSession(string addrs, SessionConfig? config)
        {
            if (config == null)
            {
                config = SessionConfig.Empty;
            }

            config = SessionConfigParser.Parse(addrs);

            Session s = new();
            for (int i = 0; i < config.Components.Count; i++)
            {
                SessionComponentInfo info = config.Components[i];

                // if addr is ~ then don't config
                if (info.ServiceName == "~")
                    continue;

                switch (info.Type)
                {
                    case ComponentType.SESSION:
                        s.Name = info.ServiceName;
                        Debug.WriteLine($"Create Session: {info.ServiceName}");
                        break;
                    case ComponentType.STREAM:
                        var stream = StreamFactory.Current.Get(info.ServiceName, info.InstanceName, info.Params);
                        Debug.WriteLine($"Create Stream: {info.ServiceName}:{info.InstanceName}");
                        s.Streams.Add(stream);
                        break;
                    case ComponentType.LOGGER:
                        var logger = LoggerFactory.Current.Get(info.ServiceName, info.InstanceName, info.Params);
                        Debug.WriteLine($"Create Logger: {info.ServiceName}:{info.InstanceName}");
                        s.Loggers.Add(logger);
                        break;
                    case ComponentType.PROTOCOL:
                        var protocol = ProtocolFactory.Current.Get(info.ServiceName, info.InstanceName, info.Params);
                        Debug.WriteLine($"Create Protocol: {info.ServiceName}:{info.InstanceName}");
                        s.Protocols.Add(protocol);
                        break;
                    case ComponentType.SERVICE:
                        var service = ServiceFactory.Current.Get(info.ServiceName, info.ServiceName, info.Params);
                        Debug.WriteLine($"Create Service: {info.ServiceName}:{info.ServiceName}");
                        s.Services.Add(service);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            s.Bind();
            Sessions.Add(s.Name, s);
            return s;
        }

        public bool TryGet(string id, out Session? session)
        {
            return Sessions.TryGetValue(id, out session);
        }
    }
}
