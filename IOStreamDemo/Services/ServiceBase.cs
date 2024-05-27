using CarrotProtocolLib.Service;
using IOStreamDemo.Protocols;
using IOStreamDemo.Streams;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Services
{
    public enum TaskServiceStatus
    {
        WaitForStart,
        Running,
        ExitSuccess,
        CancelledByCancellationTokenSource,
        ThrowException
    }

    public class ServiceBase<TReturn>
    {
        public CancellationTokenSource Cts { get; set; }
        private Task<TReturn?> TaskInstance { get; set; }
        public TReturn? ReturnValue { get; set; }
        public TaskServiceStatus Status { get; set; }
        public Exception? InternalException { get; set; }

        public ThreadPriority Priority { get; set; } = ThreadPriority.Normal;
        public TaskCreationOptions TaskOptions { get; set; } = TaskCreationOptions.None;

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


        public ServiceBase()
        {
            Cts = new();
            ReturnValue = default;
            Status = TaskServiceStatus.WaitForStart;
            InternalException = null;
        }

        public void Start()
        {
            Cts = new();
            TaskInstance = Impl(Cts.Token);
            Status = TaskServiceStatus.Running;
        }

        public void Stop()
        {
            Cts.Cancel();
            try
            {
                TaskInstance.Wait();
            }
            catch (Exception)
            {

            }
        }


        public async Task<TReturn?> Impl(CancellationToken ct)
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Hello");
                await Task.Delay(500, ct);
            }
            return await Task.FromResult(default(TReturn)!);
        }

        //public Task<TReturn?> Impl(CancellationToken ct)
        //{
        //    return Task.Run<TReturn?>(async () =>
        //    {
        //        for (int i = 0; i < 5; i++)
        //        {
        //            Console.WriteLine("Hello");
        //            await Task.Delay(500, ct);
        //        }
        //        return default;
        //    }, ct);
        //}
    }
}
