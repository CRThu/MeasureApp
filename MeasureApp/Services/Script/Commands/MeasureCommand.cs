using MeasureApp.Model.Script;
using MeasureApp.Services.ScriptLibrary;
using System;
using System.Threading.Tasks;

namespace MeasureApp.Services.Script.Commands
{
    [ScriptCommand("MEASURE")]
    public class MeasureCommand : IScriptCommand
    {
        // <measure addr="Serial::Serial::COM100" mode="DCV"/>
        // <measure addr="NiVisa::NiVisa::ASRL100::INSTR" mode="DCV"/>
        public async Task ExecuteAsync(ScriptContext context, CommandParameters parameters)
        {
            // Get "addr" parameter, if not present, try to get it from the environment default.
            string addr = parameters.Get<string>("addr");
            if (string.IsNullOrEmpty(addr))
            {
                context.Environment.TryGetValue("Env::Default::Measure", out addr);
            }

            if (string.IsNullOrEmpty(addr))
            {
                throw new InvalidOperationException("No address specified for MEASURE command and 'Env::Default::Measure' is not set.");
            }

            string mode = parameters.Get<string>("mode");
            string storeKey = parameters.Get<string>("key"); // Optional key for data storage

            await Measurement.QueryAsync(context.AppContext, addr, mode, storeKey);
        }
    }
}