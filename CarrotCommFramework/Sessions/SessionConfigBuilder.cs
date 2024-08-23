using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Sessions
{
    public interface IConfigBuilder
    {

    }

    public class SessionConfigBuilder : IConfigBuilder
    {
        public List<SessionComponentInfo> Components { get; private set; } = [];

        //public IConfigBuilder Session(string serviceName, string? instanceName = null)
        //{
        //    Components.Add(new SessionComponentInfo() { ServiceName = serviceName, InstanceName = instanceName });
        //}

        //public SessionConfig Build()
        //{
        //    return Config;
        //}
    }
}
