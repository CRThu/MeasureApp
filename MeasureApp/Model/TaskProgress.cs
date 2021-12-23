using MeasureApp.Model.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model
{
    public class TaskProgress : NotificationObjectBase
    {

        // 任务进度已用时间
        private int taskProgressElapsedTime;
        public int TaskProgressElapsedTime
        {
            get => taskProgressElapsedTime;
            set
            {
                taskProgressElapsedTime = value;
                RaisePropertyChanged(() => TaskProgressElapsedTime);
            }
        }

        // 任务进度预计时间
        private int taskProgressETATime;
        public int TaskProgressETATime
        {
            get => taskProgressETATime;
            set
            {
                taskProgressETATime = value;
                RaisePropertyChanged(() => TaskProgressETATime);
            }
        }

        // 任务进度条
        private int taskProgressStart;
        public int TaskProgressStart
        {
            get => taskProgressStart;
            set
            {
                taskProgressStart = value;
                RaisePropertyChanged(() => TaskProgressStart);
            }
        }

        private int taskProgressEnd = 100;
        public int TaskProgressEnd
        {
            get => taskProgressEnd;
            set
            {
                taskProgressEnd = value;
                RaisePropertyChanged(() => TaskProgressEnd);
            }
        }

        private int taskProgressValue;
        public int TaskProgressValue
        {
            get => taskProgressValue;
            set
            {
                taskProgressValue = value;
                RaisePropertyChanged(() => TaskProgressValue);
            }
        }

        private Stopwatch stopWatch = new();

        public long ElapsedMilliseconds => stopWatch.ElapsedMilliseconds;

        public void Start(int count, int start = 0)
        {
            TaskProgressStart = start;
            TaskProgressEnd = count - 1;
            stopWatch.Restart();
        }

        public void Stop()
        {
            stopWatch.Stop();
        }

        public void Update(int value)
        {
            TaskProgressValue = value;
            TaskProgressElapsedTime = (int)((double)stopWatch.ElapsedMilliseconds / 1000);
            TaskProgressETATime = (int)(((double)stopWatch.ElapsedMilliseconds / (value + 1) * (TaskProgressEnd + 1) - stopWatch.ElapsedMilliseconds) / 1000);
        }
    }
}
