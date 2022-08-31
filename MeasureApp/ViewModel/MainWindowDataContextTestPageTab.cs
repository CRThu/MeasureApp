using MeasureApp.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MeasureApp.Model.Common;
using System.Collections.ObjectModel;
using RoslynPad.Roslyn;
using ScottPlot;
using MeasureApp.ViewModel.Common;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        private WpfPlot plotControl = new();
        public WpfPlot PlotControl
        {
            get => plotControl;
            set
            {
                plotControl = value;
                RaisePropertyChanged(() => PlotControl);
            }
        }

        public void PlotUpdate()
        {
            double[] values = DataGen.RandomWalk(1_000_000);
            PlotControl.Plot.AddSignal(values, sampleRate: 48_000);
            PlotControl.Plot.Title("One Million Points");
            PlotControl.Refresh();
        }

        private CommandBase plotUpdateEvent;
        public CommandBase PlotUpdateEvent
        {
            get
            {
                if (plotUpdateEvent == null)
                {
                    plotUpdateEvent = new CommandBase(new Action<object>(param => PlotUpdate()));
                }
                return plotUpdateEvent;
            }
        }
    }
}
