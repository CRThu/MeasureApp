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
    	Stopwatch sw = new();
    	sw.Start();
    	dataContext.StatusBarProgressBarMax = 500-1;
        for (int i = 0; i < 500; i++)
        {
        	Thread.Sleep(10);
        	dataContext.StatusBarProgressBarValue = i;
        	dataContext.StatusBarText = $"{sw.ElapsedMilliseconds}ms";
        }
        sw.Stop();
        return 0;
    }
}
