using MeasureApp.Model.Script;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MeasureApp.Services.Script.Commands
{
    [ScriptCommand("TRIM")]
    public class TrimCommand : IScriptCommand
    {
        public Task<ExecutionDirective> ExecuteAsync(ScriptContext context, CommandParameters parameters, CancellationToken cancellationToken)
        {
            // todo
            throw new NotImplementedException();

            return Task.FromResult<ExecutionDirective>(ContinueExecution.Instance);
        }
    }
}