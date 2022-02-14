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
            //int N = 262144;
            //int fftN = (int)Math.Pow(2, Math.Ceiling(Math.Log2(N)));
            //double fin = 997.89;
            //double fs = 48000;
            //double dc = 0.2;
            //double phase = random.NextDouble() * Math.PI * 2;
            //double[] t = Enumerable.Range(0, N).Select(tx => (double)tx).ToArray();
            //double[] sine = t.Select(tx => dc + Math.Sin(2 * Math.PI * fin * tx / fs + phase) + (random.NextDouble() - 0.5) * 0.05).ToArray();

            double fs = 200000;
            double[] sine = ImportSamples.ReadFileData("../../../Model/FftAnalysis/AD7606_TestData_88d69.txt").Select(v => (double)v).ToArray();
            int fftN = (int)Math.Pow(2, Math.Floor(Math.Log2(sine.Length)));
            sine = sine.Take(fftN).ToArray();
            double[] t = Enumerable.Range(0, sine.Length).Select(t => t / fs).ToArray();

            Stopwatch stopwatch = new();
            stopwatch.Start(); //  开始监视代码运行时间

            (double[] freq, double[] mag) = FftAnalysis.FftMag(sine, fs, "HFT144D");
            double[] magNorm = FftAnalysis.NormalizedTo0dB(mag);
            double[] powerNorm = mag.Select(m => m * m).ToArray();

            stopwatch.Stop(); //  停止监视
            Debug.WriteLine($"FFT:{stopwatch.ElapsedMilliseconds}ms.");

            (double fc, int yIndex, double yMax) baseSignal = DynamicPerfAnalysis.FindMax(freq, magNorm);
            Debug.WriteLine($"Base Signal: {baseSignal.fc}Hz | {baseSignal.yMax} dB.");

            int span = 6; // 信号中心频点-span点至+span点计算相加信号功率
            int spanH1 = 3; // 谐波计算频点-spanH1点至+spanH1点寻找谐波中心频点
            int spanH2 = 1; // 谐波中心频点-spanH2点至+spanH2点计算相加信号功率
            int nHarmonic = 5; // 计算到nHarmonic次谐波

            double pDc = DynamicPerfAnalysis.SubArray(powerNorm, 0, span - 1).Sum();
            double pSignal = DynamicPerfAnalysis.SubArray(powerNorm, baseSignal.yIndex - span, baseSignal.yIndex + span).Sum();

            double[] fHarmonic = new double[nHarmonic];
            int[] fHarmonicIndex = new int[nHarmonic];
            double[] pHarmonic = new double[nHarmonic];
            double[] dBHarmonic = new double[nHarmonic];

            for (int i = 1; i <= nHarmonic; i++)
            {
                fHarmonic[i - 1] = i * baseSignal.fc;
                fHarmonicIndex[i - 1] = i * (baseSignal.yIndex - 1) + 1;

                (double fc, int yIndex, double yMax) harmonic = DynamicPerfAnalysis.FindMax(freq, powerNorm, fHarmonicIndex[i - 1] - spanH1, fHarmonicIndex[i - 1] + spanH1);

                fHarmonic[i - 1] = harmonic.fc;
                fHarmonicIndex[i - 1] = harmonic.yIndex;
                pHarmonic[i - 1] = DynamicPerfAnalysis.SubArray(powerNorm, harmonic.yIndex - spanH2, harmonic.yIndex + spanH2).Sum();

                if (i != 1)
                {
                    dBHarmonic[i - 1] = 10 * Math.Log10(pHarmonic[i - 1] / pHarmonic[0]);
                    Debug.WriteLine($"Harmonic {i}: {fHarmonic[i - 1]}Hz | {dBHarmonic[i - 1]} dB.");
                }
            }

            pHarmonic[0] = 0;

            double pDistortion = pHarmonic.Sum();
            double pNoise = powerNorm.Sum() - pDc - pSignal - pDistortion;

            double SNR = 10 * Math.Log10(pSignal / pNoise);
            double THD = 10 * Math.Log10(pDistortion / pSignal);
            double SINAD = 10 * Math.Log10(pSignal / (pNoise + pDistortion));
            double ENOB = (SINAD - 1.76) / 6.02;

            Debug.WriteLine($"SNR: {SNR} dB.");
            Debug.WriteLine($"THD: {THD} dB.");
            Debug.WriteLine($"SINAD: {SINAD} dB.");
            Debug.WriteLine($"ENOB: {ENOB} Bits.");

            ChartData.ClearData();
            switch ((string)param)
            {
                case "0":
                    ChartData.AddData(t, sine);
                    break;
                case "1":
                    ChartData.AddData(freq, magNorm);
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
