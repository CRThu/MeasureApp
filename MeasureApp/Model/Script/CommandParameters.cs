using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.Script
{
    /// <summary>
    /// Represents the parameters for a single script command, parsed from an XML-like tag.
    /// Inherits type-safe retrieval from StringParameterCollection.
    /// </summary>
    public class CommandParameters : StringParameterCollection
    {
        public string CommandName { get; }

        public CommandParameters(IDictionary<string, string> attributes)
            // Pass all attributes except "Tag" to the base class
            : base(attributes.Where(kv => !kv.Key.Equals("Tag", StringComparison.OrdinalIgnoreCase))
                             .ToDictionary(kv => kv.Key, kv => kv.Value))
        {
            // The "Tag" attribute is special and defines the command name
            if (attributes.TryGetValue("Tag", out var commandName))
            {
                CommandName = commandName;
            }
            else
            {
                throw new ArgumentException("Command attributes must contain a 'Tag' key.", nameof(attributes));
            }
        }
    }
}