using MeasureApp.Model.Script;
using System.Threading;
using System.Threading.Tasks;

namespace MeasureApp.Services.Script.Commands
{
    [ScriptCommand("DELAY")]
    public class DelayCommand : IScriptCommand
    {
        // <delay time="..."/>
        public async Task<ExecutionDirective> ExecuteAsync(ScriptContext context, CommandParameters parameters, CancellationToken cancellationToken)
        {
            // Get "time" parameter, default to 1000ms if not provided.
            int milliseconds = parameters.Get<int>("time", 1000);

            try
            {
                await Task.Delay(milliseconds, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                return StopExecution.Instance;
            }

            return ContinueExecution.Instance;
        }
    }
}