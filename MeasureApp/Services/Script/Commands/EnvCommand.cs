using MeasureApp.Model.Script;
using System;
using System.Threading.Tasks;

namespace MeasureApp.Services.Script.Commands
{
    [ScriptCommand("ENV")]
    public class EnvCommand : IScriptCommand
    {
        public Task ExecuteAsync(ScriptContext context, ScriptMethodParameters parameters)
        {
            string key = parameters.Get<string>("key");
            string value = parameters.Get<string>("value");

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Parameter 'key' cannot be null or empty for ENV command.");
            }

            // Set the environment variable in the context
            context.Environment[key] = value;

            return Task.CompletedTask;
        }
    }
}