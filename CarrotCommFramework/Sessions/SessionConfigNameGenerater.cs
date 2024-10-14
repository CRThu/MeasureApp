using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Sessions
{
    public class SessionConfigNameGenerater
    {
        public static class SessionConfigInstanceCounter
        {
            public static readonly Dictionary<string, int> AutoGenerateInstanceCount = new();
        }

        /*
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
        */
    }
}
