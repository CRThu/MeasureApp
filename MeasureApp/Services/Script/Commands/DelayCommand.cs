using MeasureApp.Model.Script;
using System.Threading.Tasks;

namespace MeasureApp.Services.Script.Commands
{
    [ScriptCommand("DELAY")]
    public class DelayCommand : IScriptCommand
    {
        public async Task<ExecutionDirective> ExecuteAsync(ScriptContext context, CommandParameters parameters)
        {
            // Get "ms" parameter, default to 1000ms if not provided.
            int milliseconds = parameters.Get<int?>("ms") ?? 1000;
            
            await Task.Delay(milliseconds);

            return ContinueExecution.Instance;
        }
    }
}