using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp
{
    /// <summary>
    /// 代码格式示例：
    /// returnValue = Func(param1, param2, ...);
    /// 函数重载不可用，泛型不可用
    /// </summary>
    public class CodeParse
    {

        public string[] Codes;
        public Dictionary<string, dynamic> ProcessVariables = new();

        public CodeParse()
        {

        }

        public CodeParse(string codes)
        {
            Codes = SplitLines(codes);
        }

        public static string[] SplitLines(string codes)
        {
            return codes.Split(";\r\n".ToArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        public void UpdateProcessVariables(string key, dynamic value)
        {
            if (!ProcessVariables.ContainsKey(key))
            {
                ProcessVariables.Add(key, value);
            }
            else
            {
                ProcessVariables[key] = value;
            }
        }

        public static (string funcName, string[] paramArray, string returnValue) CommandParse(string cmd)
        {
            string[] splitReturn = cmd.Split("=".ToCharArray()).Select(s => s.Trim()).ToArray();
            string[] cmd_params = splitReturn[^1].Split("(,)".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
            return (cmd_params[0], cmd_params[1..], splitReturn.Length == 1 ? null : splitReturn[0]);
        }

        public dynamic ExecuteAllCodes()
        {
            foreach (string code in Codes)
            {
                ExecuteFunction(code, null);
            }
            return 0;
        }

        public dynamic ExecuteAllCodes(dynamic instance)
        {
            foreach (string code in Codes)
            {
                ExecuteFunction(code, instance);
            }
            return 0;
        }

        public dynamic ExecuteFunction(string code)
        {
            return ExecuteFunction(code, null);
        }

        public dynamic ExecuteFunction(string code, dynamic instance)
        {
            (string funcName, string[] paramArray, string returnValue) command = CommandParse(code);

            Type T = typeof(GPIB3458AMeasure);
            MethodInfo method = T.GetMethod(command.funcName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (method == null)
            {
                throw new NotSupportedException($"未找到函数 '{command.funcName}'");
            }

            List<dynamic> paramArrays = new();
            for (int i = 0; i < method.GetParameters().Length; i++)
            {
                paramArrays.Add(Convert.ChangeType(command.paramArray[i], method.GetParameters()[i].ParameterType));
            }
            dynamic returnData = method.Invoke(instance, paramArrays.ToArray());
            if (returnData != null && command.returnValue != null)
            {
                UpdateProcessVariables(command.returnValue, returnData);
            }

            return returnData;
        }
    }
}
