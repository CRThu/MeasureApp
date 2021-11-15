using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Definitions.Series;
using LiveCharts.Geared;
using LiveCharts.Wpf;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 图表数据绑定
        private GearedValues<double> plotViewLineValues = new();

        public GearedValues<double> PlotViewLineValues
        {
            get => plotViewLineValues;
            set
            {
                plotViewLineValues = value;
                RaisePropertyChanged(() => PlotViewLineValues);
            }
        }

        // Mapper
        // XAML: Configuration="{Binding Config}"
        //public CartesianMapper<StringDataClass> Config
        //{
        //    get => Mappers.Xy<StringDataClass>().X(v => Convert.ToDouble(v.X)).Y(v => Convert.ToDouble(v.StringData));
        //}


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
                            PlotViewLineValues.Clear();
                            PlotViewLineValues.AddRange(DataStorageInstance.DataStorageDictionary[DataStorageSelectedValue].Select(v => Convert.ToDouble(v.Value)));
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
        public Random random = new();
         public double trend = 0;
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
                                l.Add(trend - PlotViewLineValues.Count - i);
                            }
                            PlotViewLineValues.AddRange(l);
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
