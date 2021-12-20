using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MeasureApp.Model
{
    public static class Utility
    {
        public static T TimeoutCheck<T>(int millisecondsTimeout, Func<T> func)
        {
            ManualResetEvent resetEvent = new(false);
            bool isCompleted = false;
            Task<T> task = Task.Run(() =>
            {
                T result = func.Invoke();
                isCompleted = true;
                resetEvent.Set();
                return result;
            });
            _ = resetEvent.WaitOne(millisecondsTimeout);
            return isCompleted ? task.Result : default;
        }
    }
}
