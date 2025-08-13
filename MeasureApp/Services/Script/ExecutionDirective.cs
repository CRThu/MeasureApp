using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Services.Script
{
    public abstract record ExecutionDirective;

    public sealed record ContinueExecution : ExecutionDirective
    {
        public static ContinueExecution Instance { get; } = new ContinueExecution();
        private ContinueExecution() { }
    }

    public sealed record JumpToLine : ExecutionDirective
    {
        public int TargetLine { get; set; }
        public JumpToLine(int targetLine)
        {
            TargetLine = targetLine;
        }
    }

    public sealed record StopExecution : ExecutionDirective
    {
        public static StopExecution Instance { get; } = new StopExecution();
        private StopExecution() { }
    }
}
