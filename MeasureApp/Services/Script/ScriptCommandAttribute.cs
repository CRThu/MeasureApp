using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Services.Script
{
    /// <summary>
    /// Attribute to associate a class with a script command name.
    /// This allows for automatic discovery and registration of commands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ScriptCommandAttribute : Attribute
    {
        public string CommandName { get; }

        /// <summary>
        /// Initializes a new instance of the ScriptCommandAttribute class.
        /// </summary>
        /// <param name="commandName">The name of the command tag in the script (e.g., "measure"). It is case-insensitive.</param>
        public ScriptCommandAttribute(string commandName)
        {
            CommandName = commandName.ToUpperInvariant();
        }
    }
}
