/// lang=C#
using System;
using System.Windows;
using System.Threading;
using System.Linq;
using System.Diagnostics;
using MeasureApp;
using MeasureApp.ViewModel;
using MeasureApp.Model;

public class Automation
{
    public int Main(MainWindowDataContext dataContext)
    {
        int loop = 500;
        dataContext.ProgressStopWatchStart(loop);
        for (int i = 0; i < loop; i++)
        {
            Thread.Sleep(10);
            dataContext.StatusBarText = $"{dataContext.progressStopWatch.ElapsedMilliseconds}ms";
            dataContext.ProgressStopWatchUpdate(i);
            if (dataContext.IsAutomationCancelled)
                return -1;
        }
        dataContext.ProgressStopWatchStop();
        return 0;
    }
}
