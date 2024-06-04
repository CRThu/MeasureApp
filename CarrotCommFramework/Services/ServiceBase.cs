using CarrotCommFramework.Loggers;
using CarrotCommFramework.Protocols;
using CarrotCommFramework.Streams;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Services
{
    /// <summary>
    /// 服务状态
    /// </summary>
    public enum ServiceStatus
    {
        WaitForStart,
        Running,
        Cancelling,
        ExitSuccess,
        Cancelled,
        ThrowException
    }

    /// <summary>
    /// 服务接口
    /// </summary>
    public interface IServiceBase<TReturn>
    {
        /// <summary>
        /// 服务返回值
        /// </summary>
        public TReturn? ReturnValue { get; }

        /// <summary>
        /// 服务状态
        /// </summary>
        public ServiceStatus Status { get; }

        /// <summary>
        /// 服务异常
        /// </summary>
        public Exception? InternalException { get; }

        /// <summary>
        /// 打开服务
        /// </summary>
        void Start();

        /// <summary>
        /// 停止服务
        /// </summary>
        void Stop();
    }

    /// <summary>
    /// 服务基类
    /// </summary>
    /// <typeparam name="TReturn"></typeparam>
    public abstract class ServiceBase<TReturn> : IServiceBase<TReturn>
    {
        /// <summary>
        /// 取消信号
        /// </summary>
        public CancellationTokenSource Cts { get; set; }

        /// <summary>
        /// 服务实例
        /// </summary>
        private Task<TReturn?>? Service { get; set; }

        /// <summary>
        /// 服务返回值
        /// </summary>
        public TReturn? ReturnValue { get; set; }

        /// <summary>
        /// 服务状态
        /// </summary>
        public ServiceStatus Status { get; set; }

        /// <summary>
        /// 服务异常
        /// </summary>
        public Exception? InternalException { get; set; }

        /// <summary>
        /// 是否外部请求取消任务
        /// </summary>
        public bool IsCancellationRequested
        {
            get
            {
                if (Cts.Token.IsCancellationRequested)
                    Status = ServiceStatus.Cancelling;
                return Cts.Token.IsCancellationRequested;
            }
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        public ServiceBase()
        {
            Cts = new();
            ReturnValue = default;
            Status = ServiceStatus.WaitForStart;
            InternalException = null;
        }

        /// <summary>
        /// 打开服务
        /// </summary>
        public void Start()
        {
            Cts = new();
            Service = Impl(Cts.Token);
            Status = ServiceStatus.Running;
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            Cts.Cancel();
            try
            {
                Service?.Wait();
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 服务实现方法
        /// </summary>
        /// <param name="ct">返回</param>
        /// <returns>返回值</returns>
        public virtual async Task<TReturn?> Impl(CancellationToken ct)
        {
            return await Task.FromResult<TReturn>(default(TReturn?));
        }

        //public async Task<TReturn?> Impl(CancellationToken ct)
        //{
        //    for (int i = 0; i < 5; i++)
        //    {
        //        Debug.WriteLine("Hello");
        //        await Task.Delay(500, ct);
        //    }
        //    return await Task.FromResult(default(TReturn?));
        //}

        //public async Task<TReturn?> ImplTask(CancellationToken ct)
        //{
        //    return await Task<TReturn?>.Run(() => Impl(ct), ct);
        //}
    }
}
