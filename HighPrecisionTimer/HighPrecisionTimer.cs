using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Media;

namespace HighPrecisionTimer
{
    public class HighPrecisionTimer : IDisposable
    {
        private static TIMECAPS ptc;
        private uint uDelay;
        private uint uResolution;
        private readonly LPTIMECALLBACK fptc;
        private uint uTimerID;

        public bool IsRunning { get; private set; }

        public delegate void TickEventHandler(object? sender, TickEventArgs e);
        public event TickEventHandler? Tick;

        private object lockObj = new();
        private bool disposedValue;

        public uint Interval
        {
            get
            {
                return uDelay;
            }
            set
            {
                if (value < ptc.wPeriodMin || value > ptc.wPeriodMax)
                    throw new InvalidOperationException("Invalid Interval");
                uDelay = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return IsRunning;
            }
            set
            {
                if (!IsRunning)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
        }

        static HighPrecisionTimer()
        {
            _ = PInvoke.timeGetDevCaps(ptc: out ptc, cbtc: (uint)Marshal.SizeOf(ptc));
        }

        public HighPrecisionTimer()
        {
            IsRunning = false;
            uDelay = ptc.wPeriodMin;
            uResolution = ptc.wPeriodMin;
            fptc = TimerEventCallback;
            System.Timers.Timer c = new();
        }


        public HighPrecisionTimer(uint interval) : this()
        {
            Interval = interval;
        }

        public void Start()
        {
            if (!IsRunning)
            {
                lock (lockObj)
                {
                // TIME_ONESHOT
                // TIME_PERIODIC

                // TIME_CALLBACK_FUNCTION
                // TIME_CALLBACK_EVENT_SET
                // TIME_CALLBACK_EVENT_PULSE
                // TIME_KILL_SYNCHRONOUS
                uTimerID = PInvoke.timeSetEvent(
                            uDelay: uDelay,
                            uResolution: uResolution,
                            fptc: fptc,
                            dwUser: 0,
                            fuEvent: PInvoke.TIME_PERIODIC | PInvoke.TIME_KILL_SYNCHRONOUS);
                if (uTimerID == 0)
                    throw new InvalidOperationException("Failed to initial Timer");
                IsRunning = true;
                }
            }
            else
            {
                throw new InvalidOperationException("Timer is running.");
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                lock (lockObj)
                {
                uint mmResult = PInvoke.timeKillEvent(uTimerID: uTimerID);
                if (mmResult != PInvoke.MMSYSERR_NOERROR)
                    throw new InvalidOperationException("Failed to stop Timer");
                IsRunning = false;
                }
            }
            else
            {
                throw new InvalidOperationException("Timer is not running.");
            }
        }

        private void TimerEventCallback(uint uTimerID, uint uMsg, nuint dwUser, nuint dw1, nuint dw2)
        {
            Tick?.Invoke(this, new TickEventArgs());
        }

        // TODO 内存管理可能存在问题
        public static Task Delay(int milliSeconds = 1)
        {
            if (milliSeconds < 0)
                throw new InvalidOperationException($"Invalid Delay");
            else if (milliSeconds == 0)
                milliSeconds = 1;

            HighPrecisionTimer timer = new((uint)milliSeconds);
            TaskCompletionSource taskCompletionSource = new();
            timer.Tick += (object? sender, TickEventArgs e) =>
            {
                //logger.Info($"TIMER CALLBACK");
                timer.Stop();
                //logger.Info($"TIMER STOP");
                //taskCompletionSource.TrySetResult();
                taskCompletionSource.SetResult();
            };
            //logger.Info($"TIMER CTOR");
            timer.Start();
            //logger.Info($"TIMER START id={timer.uTimerID}");
            return taskCompletionSource.Task;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    //Tick = null;
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                //_ = PInvoke.timeKillEvent(uTimerID: uTimerID);
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        ~HighPrecisionTimer()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }


        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class TickEventArgs : EventArgs
    {
        public DateTime SignalTime { get; private set; }

        public TickEventArgs()
        {
            SignalTime = DateTime.Now;
        }
    }
}
