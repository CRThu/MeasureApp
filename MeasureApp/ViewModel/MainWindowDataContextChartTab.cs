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

        private SeriesCollection chartSeriesCollection;

        public SeriesCollection ChartSeriesCollection
        {
            get => chartSeriesCollection;
            set
            {
                chartSeriesCollection = value;
                RaisePropertyChanged(() => ChartSeriesCollection);
            }
        }

        // PlotView数据刷新事件
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
                            // TEST 测试图表更新
                            _observableValues.Clear();
                            _observableValues.AddRange(DataStorageInstance.DataStorageDictionary[DataStorageSelectedValue].Select(cls => Convert.ToDouble(cls.StringData)));
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


        // PlotView大数据量测试事件
        Random random = new();
        double trend = 0;
        private CommandBase plotViewTestEvent;
        public CommandBase PlotViewTestEvent
        {
            get
            {
                if (plotViewTestEvent == null)
                {
                    plotViewTestEvent = new CommandBase(new Action<object>(param =>
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
                return plotViewTestEvent;
            }
        }
    }
}
