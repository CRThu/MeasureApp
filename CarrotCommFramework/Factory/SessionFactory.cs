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

        public SessionFactory()
        {
        }

        public Session CreateSession(string addrs, SessionConfig? config)
        {
            if (config == null)
            {
                config = SessionConfig.Empty;
            }

            SessionConfig cfg = new SessionConfig(config);

            var parseConfig = SessionConfigParser.Parse(addrs);

            for (int i = 0; i < parseConfig.Components.Count; i++)
                cfg.Add(parseConfig.Components[i]);

            Session s = new();
            for (int i = 0; i < cfg.Components.Count; i++)
            {
                SessionComponentInfo info = cfg.Components[i];

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
                        var service = ServiceFactory.Current.Get(info.ServiceName, info.InstanceName, info.Params);
                        Debug.WriteLine($"Create Service: {info.ServiceName}:{info.InstanceName}");
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
