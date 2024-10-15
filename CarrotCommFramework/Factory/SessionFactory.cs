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

            // create
            var opts = options.Flatten();
            foreach (var opt in opts)
            {
                string interf = opt.Sources.TryGetValue("interface", out string? val1) ? val1 : "null";
                string type = opt.Sources.TryGetValue("type", out string? val2) ? val2 : "default";
                string name = opt.Sources.TryGetValue("name", out string? val3) ? val3 : "unnamed";
                switch (interf)
                {
                    case "session":
                        Session s = new();
                        Sessions.Add(name, s);
                        Console.WriteLine($"Create Session: {name}");
                        break;
                    case "stream":
                        var stream = StreamFactory.Current.Get(type, name, opt.Sources);
                        Console.WriteLine($"Create Stream: {type}:{name}");
                        //s.Streams.Add(stream);
                        break;
                    case "logger":
                        var logger = LoggerFactory.Current.Get(type, name, opt.Sources);
                        Console.WriteLine($"Create Logger: {type}:{name}");
                        //s.Loggers.Add(logger);
                        break;
                    case "protocol":
                        var protocol = ProtocolFactory.Current.Get(type, name, opt.Sources);
                        Console.WriteLine($"Create Protocol: {type}:{name}");
                        //s.Protocols.Add(protocol);
                        break;
                    case "service":
                        var service = ServiceFactory.Current.Get(type, name, opt.Sources);
                        Console.WriteLine($"Create Service: {type}:{name}");
                        //s.Services.Add(service);
                        break;
                    default:
                        break;
                }
            }

            // bind
            foreach (var opt in opts)
            {
                if (opt.NestedSources.Count != 0
                    && opt.Sources.TryGetValue("interface", out string? val3)
                    && val3 == "session")
                {
                    string name1 = opt.Sources.TryGetValue("name", out string? val1) ? val1 : "unnamed";
                    Current.TryGet(name1, out Session? s);
                    foreach (var src in opt.NestedSources)
                    {
                        string interf = src.Sources.TryGetValue("interface", out string? val4) ? val4 : "null";
                        string name = src.Sources.TryGetValue("name", out string? val5) ? val5 : "unnamed";

                        switch (interf)
                        {
                            case "stream":
                                var stream = StreamFactory.Current.Get(null, name, null);
                                s.Streams.Add(stream);
                                break;
                            case "logger":
                                var logger = LoggerFactory.Current.Get(null, name, null);
                                s.Loggers.Add(logger);
                                break;
                            case "protocol":
                                var protocol = ProtocolFactory.Current.Get(null, name, null);
                                s.Protocols.Add(protocol);
                                break;
                            case "service":
                                var service = ServiceFactory.Current.Get(null, name, null);
                                s.Services.Add(service);
                                break;
                            default:
                                break;
                        }
                    }
                    s.Bind();
                    return s;
                }
            }

            return null;
        }

        public bool TryGet(string id, out Session? session)
        {
            return Sessions.TryGetValue(id, out session);
        }
    }
}
