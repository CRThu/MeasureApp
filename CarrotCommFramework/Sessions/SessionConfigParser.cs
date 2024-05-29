using CarrotCommFramework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Sessions
{
    /// <summary>
    /// 配置解析器
    /// </summary>
    public class SessionConfigParser
    {
        /// <summary>
        /// 从配置类解析
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static List<SessionComponentInfo> Parse(SessionConfig config)
        {
            List<SessionComponentInfo> infos =
            [
                .. config.PresetSessionCommands.Select(c => ParseOne(ComponentType.SESSION, c)),
                .. config.PresetStreamCommands.Select(c => ParseOne(ComponentType.STREAM, c)),
                .. config.PresetProtocolCommands.Select(c => ParseOne(ComponentType.PROTOCOL, c)),
                .. config.PresetLoggerCommands.Select(c => ParseOne(ComponentType.LOGGER, c)),
                .. config.PresetServiceCommands.Select(c => ParseOne(ComponentType.SERVICE, c)),
            ];
            return infos;
        }

        /// <summary>
        /// 从字符串命令解析
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static List<SessionComponentInfo> Parse(string command)
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

            List<SessionComponentInfo> infos = new();

            string[] commandsSplitByType = command.ToUpper().Split('+');

            for (int i = 0; i < commandsSplitByType.Length; i++)
                infos.AddRange(commandsSplitByType[i].Split(';').Select(addr => ParseOne((ComponentType)i, addr)));

            return infos;
        }

        /// <summary>
        /// 从字符串命令解析单个命令
        /// </summary>
        /// <param name="type"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static SessionComponentInfo ParseOne(ComponentType type, string command)
        {
            // SERVICE1://INSTANCE1[@PARAM1[,PARAM2]...]

            // ServiceName:     SERVICE1
            // InstanceName:    INSTANCE1
            // Params:          INSTANCE1 [,PARAM1] [,PARAM2] [...]

            SessionComponentInfo info = new()
            {
                Type = type
            };

            int idx = command.IndexOf("://");
            if (idx == -1)
            {
                info.ServiceName = command;
            }
            else
            {
                info.ServiceName = command[..idx];

                string temp = command[(idx + 3)..];
                string[] strs = temp.Split('@', 2);

                if (strs.Length == 1)
                {
                    info.InstanceName = temp;
                    info.Params = [info.InstanceName];
                }
                else
                {
                    info.InstanceName = strs[0];
                    info.Params = [.. (string[])[info.InstanceName, .. strs[1].Split(',')]];
                }

            }

            return info;
        }
    }
}
