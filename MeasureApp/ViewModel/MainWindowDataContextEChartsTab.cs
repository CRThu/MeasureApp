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

        public void UpdateEChartsTest(object param)
        {
            switch ((string)param)
            {
                case "0":
                    ChartData1 = new(new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new double[] { 1, 4, 9, 16, 25, 36, 49, 64, 81, 100 });
                    break;
                case "1":
                    ChartData2 = new(new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new double[] { 1, 0.5, 0.33, 0.25, 0.2, 0.17, 0.14, 0.13, 0.11, 0.1 });
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
