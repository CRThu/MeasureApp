using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MeasureApp.Model.SerialPortScript
{
    public static class HtmlTagUtility
    {
        public static bool IsMatchHtmlTag(string codeString)
        {
            string regexStr = @"<[^>]+>";
            return Regex.IsMatch(codeString, regexStr, RegexOptions.IgnoreCase);
        }

        public static bool IsMatchHtmlTag(string codeString, string TagName)
        {
            string regexStr = @$"<{TagName}[^>]*?>[\s\S]*?</{TagName}>";
            return Regex.IsMatch(codeString, regexStr, RegexOptions.IgnoreCase);
        }

        public static string MatchHtmlTag(string codeString, string TagName)
        {
            string regexStr = @$"<{TagName}[^>]*?>(?<Attribute>[\s\S]*)</{TagName}>";
            //string regexStr = @$"<{TagName}[^>]*?>(?<Attribute>[^<]*)</{TagName}>";
            Match match = Regex.Match(codeString, regexStr, RegexOptions.IgnoreCase);
            // Debug.WriteLine($"{match.Groups["Attribute"].Value}");
            return match.Groups["Attribute"].Value;
        }

    }
}
