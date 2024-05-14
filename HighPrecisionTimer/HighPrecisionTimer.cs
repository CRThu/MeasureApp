using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Media;

namespace HighPrecisionTimer
{
    public class HighPrecisionTimer
    {
        private static TIMECAPS ptc;
        private uint uDelay;
        private uint uResolution;
        private LPTIMECALLBACK fptc;
        private uint uTimerID;

        public bool IsRunning { get; private set; }

        public delegate void TickEventHandler(object? sender, TickEventArgs e);
        public event TickEventHandler? Tick;

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

        ~HighPrecisionTimer()
        {
            _ = PInvoke.timeKillEvent(uTimerID: uTimerID);
        }

        public void Start()
        {
            if (!IsRunning)
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

        public void Stop()
        {
            if (IsRunning)
            {
                _ = PInvoke.timeKillEvent(uTimerID: uTimerID);
                IsRunning = false;
            }
        }

        private void TimerEventCallback(uint uTimerID, uint uMsg, nuint dwUser, nuint dw1, nuint dw2)
        {
            Tick?.Invoke(this, new TickEventArgs());
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
