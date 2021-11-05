using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 图表数据绑定
        private PlotModel plotViewPlotModel = new() { Title = "图表" };
        public PlotModel PlotViewPlotModel
        {
            get => plotViewPlotModel;
            set
            {
                plotViewPlotModel = value;
                RaisePropertyChanged(() => PlotViewPlotModel);
            }
        }

        // DataPoints
        private ObservableCollection<DataPoint> plotViewDataPoints = new();
        public ObservableCollection<DataPoint> PlotViewDataPoints
        {
            get => plotViewDataPoints;
            set
            {
                plotViewDataPoints = value;
                RaisePropertyChanged(() => PlotViewDataPoints);
            }
        }

        // PlotView数据刷新事件
        // TODO
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
                            for (int i = 0; i < DataStorageDataGridBinding.Count; i++)
                            {
                                PlotViewDataPoints.Add(new DataPoint(i + 1, Convert.ToDouble(DataStorageDataGridBinding[i].StringData)));
                            }
                            PlotViewPlotModel.InvalidatePlot(true);
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
