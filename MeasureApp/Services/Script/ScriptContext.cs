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
        public ScriptExecutor Executor { get; }

        public ScriptContext(AppContextManager appContext, ScriptEnvironment environment, ScriptExecutor executor)
        {
            AppContext = appContext;
            Environment = environment;
            Executor = executor;
        }
    }
}
