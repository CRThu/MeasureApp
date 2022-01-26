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
        private EChartsLineData chartData1 = new();
        public EChartsLineData ChartData1
        {
            get => chartData1;
            set
            {
                chartData1 = value;
                RaisePropertyChanged(() => ChartData1);
            }
        }

        private EChartsLineData chartData2 = new();
        public EChartsLineData ChartData2
        {
            get => chartData2;
            set
            {
                chartData2 = value;
                RaisePropertyChanged(() => ChartData2);
            }
        }

        public void UpdateEChartsTest(object param)
        {
            switch ((string)param)
            {
                case "0":
                    ChartData1.AddData(ChartData1.Count, (random.NextDouble() - 0.5) * 5 + ChartData1.Count);
                    break;
                case "1":
                    List<double> l = Enumerable.Range(ChartData2.Count, 1000).Select(n => (double)n).ToList();
                    ChartData2.AddData(l, l.Select(lx => (random.NextDouble() - 0.5) * 50 + lx));
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
