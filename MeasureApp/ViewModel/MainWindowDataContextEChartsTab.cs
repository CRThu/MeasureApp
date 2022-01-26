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
        // ECharts数据绑定
        private EChartsLineData chartData = new();
        public EChartsLineData ChartData
        {
            get => chartData;
            set
            {
                chartData = value;
                RaisePropertyChanged(() => ChartData);
            }
        }

        public void UpdateEChartsTest(object param)
        {
            int N = 2048;
            double fin = 997.89;
            double fs = 48000;
            double phase = random.NextDouble() * Math.PI * 2;
            ChartData.ClearData();
            List<double> t = Enumerable.Range(0, N).Select(tx => (double)tx).ToList();
            List<double> sine = t.Select(tx => Math.Sin(2 * Math.PI * fin * tx / fs + phase) + (random.NextDouble() - 0.5) * 0.05).ToList();

            var rfft = new NWaves.Transforms.RealFft64(N);
            double[] real = sine.ToArray();
            double[] imag = new double[N];

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //  开始监视代码运行时间
            rfft.Direct(real, real, imag);
            stopwatch.Stop(); //  停止监视
            Debug.WriteLine($"FFT:{stopwatch.ElapsedMilliseconds}ms.");

            List<double> f = t.Select(t => t / N * fs).Take(N/2+1).ToList();
            List<double> mag = new();
            for (int i = 0; i < N / 2 + 1; i++)
                mag.Add(Math.Sqrt(real[i] * real[i] + imag[i] * imag[i]));

            switch ((string)param)
            {
                case "0":
                    ChartData.AddData(t, sine);
                    break;
                case "1":
                    ChartData.AddData(f, mag);
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
