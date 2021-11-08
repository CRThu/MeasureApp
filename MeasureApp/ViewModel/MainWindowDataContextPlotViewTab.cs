using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Geared;
using LiveCharts.Wpf;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 图表数据绑定
        private GearedValues<double> _observableValues = new();

        private SeriesCollection series;

        public SeriesCollection Series
        {
            get => series;
            set
            {
                series = value;
                RaisePropertyChanged(() => Series);
            }
        }

        // PlotView数据刷新事件
        // TODO
        Random random = new();
        double trend = 0;
        private CommandBase plotViewPlotRefreshEvent;
        public CommandBase PlotViewPlotRefreshEvent
        {
            get
            {
                if (plotViewPlotRefreshEvent == null)
                {
                    plotViewPlotRefreshEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            List<double> l = new();
                            for (int i = 0; i < 500000; i++)
                            {
                                trend += ((random.NextDouble() - 0.5) / 6) + 1;
                                l.Add(trend - _observableValues.Count - i);
                            }
                            _observableValues.AddRange(l);
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return plotViewPlotRefreshEvent;
            }
        }
    }
}
