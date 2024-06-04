using CarrotCommFramework.Sessions;
using System.Collections.Generic;
using System.Diagnostics;

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

            List<SessionComponentInfo> addrInfo = SessionConfigParser.Parse(config);
            addrInfo.AddRange(SessionConfigParser.Parse(addrs));


            Session s = new();
            for (int i = 0; i < addrInfo.Count; i++)
            {
                SessionComponentInfo info = addrInfo[i];

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
