using MeasureApp.Model;
using MeasureApp.Model.Common;
using MeasureApp.Model.Converter;
using MeasureApp.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 串口选择
        private EChartsLineData chartData1;
        public EChartsLineData ChartData1
        {
            get => chartData1;
            set
            {
                chartData1 = value;
                RaisePropertyChanged(() => ChartData1);
            }
        }

        private EChartsLineData chartData2;
        public EChartsLineData ChartData2
        {
            get => chartData2;
            set
            {
                chartData2 = value;
                RaisePropertyChanged(() => ChartData2);
            }
        }

        List<double> x = new();
        List<double> y1 = new();
        List<double> y2 = new();
        public void UpdateEChartsTest(object param)
        {
            int cnt = 500000;
            x.AddRange(Enumerable.Range(x.Count, cnt).Select(n => (double)n).ToList());
            y1 = new(x.Select(x => x * x));
            y2 = new(x.Select(x => Math.Sqrt(x)));
            switch ((string)param)
            {
                case "0":
                    ChartData1 = new(x.ToArray(), y1.ToArray());
                    break;
                case "1":
                    ChartData2 = new(x.ToArray(), y2.ToArray());
                    break;
                default:
                    throw new NotImplementedException();
            }
        }


        // CommandBase
        private CommandBase updateEChartsTestEvent;
        public CommandBase UpdateEChartsTestEvent
        {
            get
            {
                if (updateEChartsTestEvent == null)
                {
                    updateEChartsTestEvent = new CommandBase(new Action<object>(param => UpdateEChartsTest(param)));
                }
                return updateEChartsTestEvent;
            }
        }
    }
}
