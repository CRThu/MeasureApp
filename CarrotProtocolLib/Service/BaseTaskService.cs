using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Service
{
    public enum TaskServiceStatus
    {
        WaitForStart,
        Running,
        ExitSuccess,
        CancelledByCancellationTokenSource,
        ThrowException
    }

    /// <summary>
    /// 通过Task运行服务,派生类需override ServiceLoop函数
    /// </summary>
    /// <typeparam name="TReturn">返回值类型</typeparam>
    public class BaseTaskService<TReturn> : IService
    {
        private CancellationTokenSource? Cts { get; set; }
        private Task<TReturn?>? TaskInstance { get; set; }
        public TReturn? ReturnValue { get; set; }
        public TaskServiceStatus Status { get; set; }
        public Exception? InternalException { get; set; }

        /// <summary>
        /// 是否外部请求取消任务属性
        /// </summary>
        public bool IsCancellationRequested
        {
            get
            {
                if (Cts.Token.IsCancellationRequested)
                    Status = TaskServiceStatus.CancelledByCancellationTokenSource;
                return Cts.Token.IsCancellationRequested;
            }
        }

        public BaseTaskService()
        {
            Cts = new();
            TaskInstance = null;
            ReturnValue = default;
            Status = TaskServiceStatus.WaitForStart;
            InternalException = null;
        }

        public void Start()
        {
            Cts = new();
            TaskInstance = Task.Run(() => TaskRunLoop(), Cts.Token);
            Status = TaskServiceStatus.Running;
        }

        public void Stop()
        {
            Cts.Cancel();
            TaskInstance.Wait();
        }

        private TReturn? TaskRunLoop()
        {
            try
            {
                var ret = ServiceLoop();
                ReturnValue = ret;
                Status = TaskServiceStatus.ExitSuccess;
                return ret;
            }
            catch (Exception e)
            {
                InternalException = e;
                Status = TaskServiceStatus.ThrowException;
                return default;
            }
        }

        public virtual TReturn? ServiceLoop()
        {
            return default;
        }
    }
}
