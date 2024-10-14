using CarrotCommFramework.Sessions;
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

        public Session CreateSession(string addrs, Options options)
        {
            Session s = new();
            /*
            for (int i = 0; i < cfg.Components.Count; i++)
            {
                SessionComponentInfo info = cfg.Components[i];

                switch (info.Type)
                {
                    case "session":
                        s.Name = info.ServiceName;
                        Console.WriteLine($"Create Session: {info.ServiceName}");
                        break;
                    case "stream":
                        var stream = StreamFactory.Current.Get(info.ServiceName, info.InstanceName, info.Params);
                        Console.WriteLine($"Create Stream: {info.ServiceName}:{info.InstanceName}");
                        s.Streams.Add(stream);
                        break;
                    case "logger":
                        var logger = LoggerFactory.Current.Get(info.ServiceName, info.InstanceName, info.Params);
                        Console.WriteLine($"Create Logger: {info.ServiceName}:{info.InstanceName}");
                        s.Loggers.Add(logger);
                        break;
                    case "protocol":
                        var protocol = ProtocolFactory.Current.Get(info.ServiceName, info.InstanceName, info.Params);
                        Console.WriteLine($"Create Protocol: {info.ServiceName}:{info.InstanceName}");
                        s.Protocols.Add(protocol);
                        break;
                    case "service":
                        var service = ServiceFactory.Current.Get(info.ServiceName, info.InstanceName, info.Params);
                        Console.WriteLine($"Create Service: {info.ServiceName}:{info.InstanceName}");
                        s.Services.Add(service);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            s.Bind();
            Sessions.Add(s.Name, s);
            */
            return s;
        }

        public bool TryGet(string id, out Session? session)
        {
            return Sessions.TryGetValue(id, out session);
        }
    }
}
