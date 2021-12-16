using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MeasureApp.Model.SerialPortScript
{
    public static class XmlTag
    {
        public static bool IsMatchXmlTag(string codeString)
        {
            // TEST: https://regex101.com/r/YZquP9/1
            // <TAG [Attr1="x"] [Attr2="x"] />
            string regexStr = @"<\w+.*\/>";
            return Regex.IsMatch(codeString, regexStr, RegexOptions.IgnoreCase);
        }

        public static Dictionary<string, string> GetXmlTagAttrs(string codeString)
        {
            // TEST: https://regex101.com/r/RTxDm6/1
            // <TAG [Attr1="x"] [Attr2="x"] />
            Dictionary<string, string> Attrs = new();
            string regexStr = @"<(?<Tag>\w+)(?<Attrs>.*)\/>";
            Match match = Regex.Match(codeString, regexStr, RegexOptions.IgnoreCase);
            Attrs.Add("Tag", match.Groups["Tag"].Value);
            Debug.WriteLine($"Attrs: {match.Groups["Attrs"].Value}");
            string[] attrsArray = match.Groups["Attrs"].Value.Trim().Replace("\"", "").Split(" =".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (attrsArray.Length % 2 != 0)
                throw new ArgumentException("Elements of attrsArray are not pairs.");
            for (int i = 0; i < attrsArray.Length; i += 2)
                Attrs.Add(attrsArray[i].ToLower(), attrsArray[i + 1]);
            return Attrs;
        }
    }
}
