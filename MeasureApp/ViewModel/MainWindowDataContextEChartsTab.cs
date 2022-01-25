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
        private string chartData1;
        public string ChartData1
        {
            get => chartData1;
            set
            {
                chartData1 = value;
                RaisePropertyChanged(() => ChartData1);
            }
        }

        private string chartData2;
        public string ChartData2
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
            string js;
            switch ((string)param)
            {
                case "0":
                    js = "{Title:\"Chart\",Data:{\"x\":[1,2,3,4,5,6,7,8,9,10],\"y\":[1,4,9,16,25,36,49,64,81,100]},Series:{Name:\"a\",Type:\"line\",DataXColumnsName:\"x\",DataYColumnsName:\"y\"}}";
                    ChartData1 = js;
                    break;
                case "1":
                    js = "{Title:\"Chart\",Data:{\"x\":[1,2,3,4,5,6,7,8,9,10],\"y\":[1,0.5,0.33,0.25,0.2,0.17,0.14,0.13,0.11,0.1]},Series:{Name:\"a\",Type:\"line\",DataXColumnsName:\"x\",DataYColumnsName:\"y\"}}";
                    ChartData2 = js;
                    break;
                default:
                    throw new NotImplementedException();
            }
            EChartsLineData eChartsLineData = new(new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new double[] { 1, 0.5, 0.33, 0.25, 0.2, 0.17, 0.14, 0.13, 0.11, 0.1 });
            Debug.WriteLine(eChartsLineData.ToJson());
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
