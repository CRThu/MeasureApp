/// lang=C#
using System;
using System.Windows;
using System.Linq;
using MeasureApp;
using System.Collections.Generic;
using MeasureApp.ViewModel;
using MeasureApp.Model;

public class Automation
{
    public int Main(MainWindowDataContext dataContext)
    {
   		List<double> l = new();
     	for (int i = 0; i < 50000; i++)
     	{
        	dataContext.trend += ((dataContext.random.NextDouble() - 0.5) / 6) + 1;
       		l.Add(dataContext.trend - dataContext.PlotViewLineValues.Count - i);
     	}
    	dataContext.PlotViewLineValues.AddRange(l);
        return  dataContext.PlotViewLineValues.Count;
    }
}
