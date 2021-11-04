using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 图表数据绑定
        private PlotModel plotViewPlotModel = new() { Title = "PlotView" };
        public PlotModel PlotViewPlotModel
        {
            get => plotViewPlotModel;
            set
            {
                plotViewPlotModel = value;
                RaisePropertyChanged(() => PlotViewPlotModel);
            }
        }
    }
}
