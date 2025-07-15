using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MeasureApp.ViewModel.Common
{
    public static class DelayEx
    {
        // Here is a possible implementation of an interruptible delay function in C#:
        // The function takes an integer parameter representing the delay time in milliseconds
        // It also takes a CancellationToken parameter, which can be used to cancel the delay
        // If the CancellationToken is cancelled before the delay time elapses, the function throws a TaskCanceledException
        // Otherwise, it waits for the specified delay time using Task.Delay

        public async static Task InterruptibleDelay(int delayTime, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(delayTime, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // The delay was cancelled
                // Handle the cancellation here
            }
        }

    }
}
