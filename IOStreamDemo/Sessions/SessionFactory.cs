﻿using IOStreamDemo.Loggers;
using IOStreamDemo.Protocols;
using IOStreamDemo.Streams;
using System.Collections.Generic;

namespace IOStreamDemo.Sessions
{
    public enum AddrType
    {
        SESSION,
        STREAM,
        LOGGER,
        PROTOCOL
    }

    public class AddrInfo
    {
        public AddrType Index { get; set; }
        public string ServiceKey { get; set; }
        public string InstanceKey { get; set; }
        public string[] Params { get; set; } = [];


        public AddrInfo(AddrType index, string addr)
        {
            // SERVICE1://INSTANCE1[@PARAM1[,PARAM2]...]

            Index = index;

            int idx = addr.IndexOf("://");

            ServiceKey = addr[..idx];

            string temp = addr[(idx + 2)..];
            string[] strs = temp.Split('@', 2);

            if (strs.Length == 1)
            {
                InstanceKey = temp;
            }
            else
            {
                InstanceKey = strs[0];
                Params = [.. (string[])[InstanceKey, .. strs[1].Split(',')]];
            }

        }
    }

    public class SessionFactory
    {
        private static readonly SessionFactory current = new();
        public static SessionFactory Current => current;

        public Dictionary<string, Session> Sessions { get; private set; } = new();

        public SessionFactory()
        {
        }

        public static List<AddrInfo> ParseAddress(string addr)
        {
            // SESSIONID
            // +SERVICE1://INSTANCE1[@PARAM1[,PARAM2]...][;SERVICE2://INSTANCE2[@PARAM1[,PARAM2]...]]
            // +SERVICE3://INSTANCE3[@PARAM1[,PARAM2]...][;SERVICE4://INSTANCE4[@PARAM1[,PARAM2]...]]
            // +SERVICE5://INSTANCE5[@PARAM1[,PARAM2]...][;SERVICE6://INSTANCE6[@PARAM1[,PARAM2]...]]

            // 范例

            // COM://7@COM7,9600,8,N,1
            // TCP://127.0.0.1:8888

            // LOGGER
            // CONSOLE://1
            // CONSOLE://1@CH1;CONSOLE://2@CH2

            List<AddrInfo> AddrInfos = new();

            string[] addrs = addr.ToUpper().Split('+');

            if (addrs.Length != 4)
                throw new KeyNotFoundException("地址解析错误,未解析四个地址");

            for (int i = 0; i < addrs.Length; i++)
                AddrInfos.AddRange(addrs[i].Split(';').Select(addr => new AddrInfo((AddrType)i, addr)));

            return AddrInfos;
        }

        public Session CreateSession(string addrs)
        {
            List<AddrInfo> addrInfo = ParseAddress(addrs);

            Session s = new();
            for (int i = 0; i < addrInfo.Count; i++)
            {
                AddrInfo info = addrInfo[i];
                switch (info.Index)
                {
                    case AddrType.SESSION:
                        s.Name = info.InstanceKey;
                        break;
                    case AddrType.STREAM:
                        var stream = StreamFactory.Current.Get(info.ServiceKey, info.InstanceKey, info.Params);
                        s.Streams.Add(stream);
                        break;
                    case AddrType.LOGGER:
                        var logger = LoggerFactory.Current.Get(info.ServiceKey, info.InstanceKey, info.Params);
                        s.Loggers.Add(logger);
                        break;
                    case AddrType.PROTOCOL:
                        var protocol = ProtocolFactory.Current.Get(info.ServiceKey, info.ServiceKey, info.Params);
                        s.Protocols.Add(protocol);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            Sessions.Add(s.Name, s);
            return s;
        }

        public bool TryGet(string id, out Session? session)
        {
            return Sessions.TryGetValue(id, out session);
        }
    }
}