using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Util
{
    public class TimeoutException : Exception
    {
        public TimeoutException(string message) : base(message) { }
    }

    public static class TimeoutDecorator
    {
        public static void TimeoutWrapper(Action func, int timeout)
        {
            var task = Task.Run(func);
            if (!task.Wait(timeout))
            {
                throw new TimeoutException("Timeout exceeded");
            }
        }

        public static TResult TimeoutWrapper<TResult>(Func<TResult> func, int timeout = 100)
        {
            var task = Task.Run(func);
            if (!task.Wait(timeout))
            {
                throw new TimeoutException("Timeout exceeded");
            }
            return task.Result;
        }
    }
}
