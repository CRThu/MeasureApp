using MeasureApp.Model.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Services.Script
{
    /// <summary>
    /// Defines the context for script command execution.
    /// It provides access to application services and script environment.
    /// </summary>
    public class ScriptContext
    {
        public AppContextManager AppContext { get; }
        public ScriptEnvironment Environment { get; }

        public ScriptContext(AppContextManager appContext, ScriptEnvironment environment)
        {
            AppContext = appContext;
            Environment = environment;
        }
    }

    /// <summary>
    /// Represents an interface for a script command that can be executed.
    /// </summary>
    public interface IScriptCommand
    {
        /// <summary>
        /// Executes the command asynchronously.
        /// </summary>
        /// <param name="context">The execution context, providing access to app services and environment variables.</param>
        /// <param name="parameters">A dictionary of parameters for the command, parsed from the script line.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ExecuteAsync(ScriptContext context, CommandParameters parameters);
    }
}
