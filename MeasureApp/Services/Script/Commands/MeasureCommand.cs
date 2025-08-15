using MeasureApp.Model.Script;
using MeasureApp.Services.ScriptLibrary;
using ScottPlot.TickGenerators.TimeUnits;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MeasureApp.Services.Script.Commands
{
    [ScriptCommand("MEASURE")]
    public class MeasureCommand : IScriptCommand
    {
        // <measure addr="Serial::Serial::COM100" mode="DCV"/>
        // <measure addr="NiVisa::NiVisa::ASRL100::INSTR" mode="DCV"/>
        public async Task<ExecutionDirective> ExecuteAsync(ScriptContext context, CommandParameters parameters, CancellationToken cancellationToken)
        {
            // Get "addr" parameter, if not present, try to get it from the environment default.
            string addr = parameters.Get<string>("addr");
            if (string.IsNullOrEmpty(addr))
            {
                addr = context.Environment.Get<string>("Env::Default::Measure");
            }

            if (string.IsNullOrEmpty(addr))
            {
                throw new InvalidOperationException("No address specified for MEASURE command and 'Env::Default::Measure' is not set.");
            }

            string mode = parameters.Get<string>("mode");
            string storeKey = parameters.Get<string>("key"); // Optional key for data storage

            try
            {
                await ScpiMeasure.QueryAsync(context.AppContext, addr, mode, storeKey, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                return StopExecution.Instance;
            }

            return ContinueExecution.Instance;
        }
    }
}