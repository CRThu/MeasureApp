using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Services.Script
{
    public abstract record ControlFlowState(int StartLine);
    public sealed record ForLoopState(int StartLine, string Variable, double End, double Step) : ControlFlowState(StartLine);
    public sealed record IfState(int StartLine, bool ConditionMet, bool ElseBlockExists) : ControlFlowState(StartLine);

}
