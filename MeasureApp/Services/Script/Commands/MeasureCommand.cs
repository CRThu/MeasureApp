using MeasureApp.Model.Script;
using MeasureApp.Services.ScriptLibrary;
using System;
using System.Threading;
using System.Threading.Tasks;
using CarrotLink.Core.Session;

namespace MeasureApp.Services.Script.Commands
{
    [ScriptCommand("MEASURE")]
    public class MeasureCommand : IScriptCommand
    {
        // <measure addr="COM100" cmd="DCV"/>
        // <measure addr="ASRL100::INSTR" cmd="DCV"/>
        /*
            <measure cmd="*RST" mode="write"/>
            <measure addr="USB0::0x0699::0x0413::C012473::INSTR" cmd="*IDN?"/>
            <measure addr="USB0::0x0699::0x0413::C012473::INSTR" cmd="MEASU:MEAS1:VAL?"/>
         */
        public async Task<ExecutionDirective> ExecuteAsync(ScriptContext context, CommandParameters parameters, CancellationToken cancellationToken)
        {
            // Get "addr" parameter, if not present, try to get it from the environment default.
            string addr = parameters.Get<string>("addr");
            if (string.IsNullOrEmpty(addr))
            {
                addr = context.Environment.Get<string>(ScriptExecutor.EnvDefaultMeasureName);
            }

            if (string.IsNullOrEmpty(addr))
            {
                throw new InvalidOperationException("No address specified for MEASURE command and 'Env::Default::Measure' is not set.");
            }

            string cmd = parameters.Get<string>("cmd");
            string storeKey = parameters.Get<string>("key"); // Optional key for data storage
            string mode = parameters.Get<string>("mode") ?? "query";

            try
            {
                switch (mode)
                {
                    case "query":
                        await ScpiMeasure.QueryAsync(context.AppContext, addr, cmd, storeKey, cancellationToken);
                        break;
                    case "write":
                        await context.AppContext.Devices[addr].SendAscii(cmd + "\n");
                        break;
                }
            }
            catch (TaskCanceledException)
            {
                return StopExecution.Instance;
            }

            return ContinueExecution.Instance;
        }
    }
}