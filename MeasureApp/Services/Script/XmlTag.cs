using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MeasureApp.Services.Script
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
            //Debug.WriteLine($"Attrs: {match.Groups["Attrs"].Value}");

            string attrsString = match.Groups["Attrs"].Value.Trim();
            int attrsStringCursor = 0;
            while (attrsStringCursor < attrsString.Length)
            {
                int index1 = attrsString.IndexOf('=', attrsStringCursor);
                if (index1 < 0)
                    break;
                int index2 = attrsString.IndexOf('"', index1 + 1);
                if (index2 < 0)
                    break;
                int index3 = attrsString.IndexOf('"', index2 + 1);
                if (index3 < 0)
                    break;
                string attr = attrsString[attrsStringCursor..index1].Trim();
                string value = attrsString[(index2+1)..index3].Trim();
                Attrs.Add(attr, value);
                attrsStringCursor = index3 + 1;
            }
            return Attrs;
        }
    }
}
