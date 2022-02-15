using MeasureApp.Model;
using MeasureApp.Model.Common;
using MeasureApp.Model.Converter;
using MeasureApp.Model.FftAnalysis;
using MeasureApp.ViewModel.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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

        private string fftAnalysisSampleFileName;
        public string FftAnalysisSampleFileName
        {
            get => fftAnalysisSampleFileName;
            set
            {
                fftAnalysisSampleFileName = value;
                RaisePropertyChanged(() => FftAnalysisSampleFileName);
            }
        }

        private FftAnalysisPropertyConfigs fftAnalysisPropertyConfig = new();
        public FftAnalysisPropertyConfigs FftAnalysisPropertyConfig
        {
            get => fftAnalysisPropertyConfig;
            set
            {
                fftAnalysisPropertyConfig = value;
                RaisePropertyChanged(() => FftAnalysisPropertyConfig);
            }
        }

        private ObservableCollection<FftAnalysisReport> fftAnalysisReports = new();
        public ObservableCollection<FftAnalysisReport> FftAnalysisReports
        {
            get => fftAnalysisReports;
            set
            {
                fftAnalysisReports = value;
                RaisePropertyChanged(() => FftAnalysisReports);
            }
        }

        private void FftAnalysisLoadSampleFile()
        {
            try
            {
                // Open File Dialog
                OpenFileDialog openFileDialog = new()
                {
                    Title = "Open Sample File...",
                    Filter = "Text File|*.txt",
                    InitialDirectory = Properties.Settings.Default.DefaultDirectory
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    Properties.Settings.Default.DefaultDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                    FftAnalysisSampleFileName = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        public void UpdateEChartsTest(object param)
        {
            try
            {
                double[] sine = File.ReadAllLines(FftAnalysisSampleFileName).Select(v => (double)BaseConverter.BaseConverterUInt64(v, 16)).ToArray();
                //FftAnalysisPropertyConfig.FftN = (int)Math.Pow(2, Math.Floor(Math.Log2(sine.Length)));
                sine = sine.Take(FftAnalysisPropertyConfig.FftN).ToArray();
                double[] t = Enumerable.Range(0, sine.Length).Select(t => t / FftAnalysisPropertyConfig.Fs).ToArray();

                Stopwatch stopwatch = new();
                stopwatch.Start(); //  开始监视代码运行时间
                FftAnalysisReports.Clear();

                (double[] freq, double[] mag) = FftAnalysis.FftMag(sine, FftAnalysisPropertyConfig.Fs, FftAnalysisPropertyConfig.Window);
                double[] magNorm = FftAnalysis.NormalizedTo0dB(mag);
                double[] powerNorm = mag.Select(m => m * m).ToArray();

                stopwatch.Stop(); //  停止监视
                Debug.WriteLine($"FFT:{stopwatch.ElapsedMilliseconds}ms.");

                (double fc, int yIndex, double yMax) baseSignal = DynamicPerfAnalysis.FindMax(freq, magNorm);
                FftAnalysisReports.Add(new FftAnalysisReport("信号", "基频", string.Format("{0:F3} Hz", baseSignal.fc), string.Format("{0:F3} dB", baseSignal.yMax)));

                //int span = 6; // 信号中心频点-span点至+span点计算相加信号功率
                //int spanH1 = 3; // 谐波计算频点-spanH1点至+spanH1点寻找谐波中心频点
                //int spanH2 = 1; // 谐波中心频点-spanH2点至+spanH2点计算相加信号功率
                //int nHarmonic = 5; // 计算到nHarmonic次谐波

                double pDc = DynamicPerfAnalysis.SubArray(powerNorm, 0, FftAnalysisPropertyConfig.SpanSignalEnergy - 1).Sum();
                double pSignal = DynamicPerfAnalysis.SubArray(powerNorm,
                    baseSignal.yIndex - FftAnalysisPropertyConfig.SpanSignalEnergy,
                    baseSignal.yIndex + FftAnalysisPropertyConfig.SpanSignalEnergy).Sum();

                double[] fHarmonic = new double[FftAnalysisPropertyConfig.NHarmonic];
                int[] fHarmonicIndex = new int[FftAnalysisPropertyConfig.NHarmonic];
                double[] pHarmonic = new double[FftAnalysisPropertyConfig.NHarmonic];
                double[] dBHarmonic = new double[FftAnalysisPropertyConfig.NHarmonic];

                for (int i = 1; i <= FftAnalysisPropertyConfig.NHarmonic; i++)
                {
                    fHarmonic[i - 1] = i * baseSignal.fc;
                    fHarmonicIndex[i - 1] = i * (baseSignal.yIndex - 1) + 1;

                    (double fc, int yIndex, double yMax) harmonic = DynamicPerfAnalysis.FindMax(freq, powerNorm,
                        fHarmonicIndex[i - 1] - FftAnalysisPropertyConfig.SpanHarmonicPeak,
                        fHarmonicIndex[i - 1] + FftAnalysisPropertyConfig.SpanHarmonicPeak);

                    fHarmonic[i - 1] = harmonic.fc;
                    fHarmonicIndex[i - 1] = harmonic.yIndex;
                    pHarmonic[i - 1] = DynamicPerfAnalysis.SubArray(powerNorm,
                        harmonic.yIndex - FftAnalysisPropertyConfig.SpanHarmonicEnergy,
                        harmonic.yIndex + FftAnalysisPropertyConfig.SpanHarmonicEnergy).Sum();

                    if (i != 1)
                    {
                        dBHarmonic[i - 1] = 10 * Math.Log10(pHarmonic[i - 1] / pHarmonic[0]);
                        FftAnalysisReports.Add(new FftAnalysisReport("信号", "谐波" + i, string.Format("{0:F3} Hz", fHarmonic[i - 1]), string.Format("{0:F3} dB", dBHarmonic[i - 1])));
                    }
                }

                pHarmonic[0] = 0;

                double pDistortion = pHarmonic.Sum();
                double pNoise = powerNorm.Sum() - pDc - pSignal - pDistortion;

                double SNR = 10 * Math.Log10(pSignal / pNoise);
                double THD = 10 * Math.Log10(pDistortion / pSignal);
                double SINAD = 10 * Math.Log10(pSignal / (pNoise + pDistortion));
                double ENOB = (SINAD - 1.76) / 6.02;

                FftAnalysisReports.Add(new FftAnalysisReport("性能", "SNR", string.Format("{0:F3} dB",SNR)));
                FftAnalysisReports.Add(new FftAnalysisReport("性能", "THD", string.Format("{0:F3} dB", THD)));
                FftAnalysisReports.Add(new FftAnalysisReport("性能", "SINAD", string.Format("{0:F3} dB", SINAD)));
                FftAnalysisReports.Add(new FftAnalysisReport("性能", "ENOB", string.Format("{0:F3} Bits", ENOB)));

                ChartData.SetSampling(FftAnalysisPropertyConfig.IsSampling || FftAnalysisPropertyConfig.FftN >= FftAnalysisPropertyConfig.AutoSamplingOnDataLength);
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
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
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

        private CommandBase fftAnalysisLoadSampleFileEvent;
        public CommandBase FftAnalysisLoadSampleFileEvent
        {
            get
            {
                if (fftAnalysisLoadSampleFileEvent == null)
                {
                    fftAnalysisLoadSampleFileEvent = new CommandBase(new Action<object>(param => FftAnalysisLoadSampleFile()));
                }
                return fftAnalysisLoadSampleFileEvent;
            }
        }
    }
}
