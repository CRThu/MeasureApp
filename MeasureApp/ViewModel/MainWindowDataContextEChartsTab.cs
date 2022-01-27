using MeasureApp.Model;
using MeasureApp.Model.Common;
using MeasureApp.Model.Converter;
using MeasureApp.Model.FftAnalysis;
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
            int fftN = (int)Math.Pow(2, Math.Ceiling(Math.Log2(N)));
            double fin = 997.89;
            double fs = 48000;
            double phase = random.NextDouble() * Math.PI * 2;
            ChartData.ClearData();
            double[] t = Enumerable.Range(0, N).Select(tx => (double)tx).ToArray();
            double[] sine = t.Select(tx => Math.Sin(2 * Math.PI * fin * tx / fs + phase) + (random.NextDouble() - 0.5) * 0.05).ToArray();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //  开始监视代码运行时间

            double[] win = FftWindow.BlackmanHarrisWindow(sine.Length);
            double[] sineWithWin = new double[sine.Length];
            for (int i = 0; i < sine.Length; i++)
                sineWithWin[i] = sine[i] * win[i];

            (double[] freq, double[] mag) = FftAnalysis.FftMag(sineWithWin, fs);

            stopwatch.Stop(); //  停止监视
            Debug.WriteLine($"FFT:{stopwatch.ElapsedMilliseconds}ms.");


            switch ((string)param)
            {
                case "0":
                    ChartData.AddData(t, sineWithWin);
                    break;
                case "1":
                    ChartData.AddData(freq, mag);
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
