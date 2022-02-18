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
                string[] samplesStr = File.ReadAllLines(FftAnalysisSampleFileName);
                double[] samples = DataDecode.Decode(samplesStr, FftAnalysisPropertyConfig);

                (double[] t, double[] v, double[] f, double[] p,
                    Dictionary<string, (double v1, string s1)> perfInfo,
                    Dictionary<string, (double v1, string s1, double v2, string s2)> sgnInfo) fftAnaylsisResult;

                // todo
                if (FftAnalysisPropertyConfig.Mode == "EnergyCorrection")
                    fftAnaylsisResult = DynamicPerfAnalysis.FftAnalysisEnergyCorrection(samples, FftAnalysisPropertyConfig);
                else if (FftAnalysisPropertyConfig.Mode == "AmplitudeCorrection")
                    fftAnaylsisResult = DynamicPerfAnalysis.FftAnalysisAmplitudeCorrection(samples, FftAnalysisPropertyConfig);
                else
                    throw new NotImplementedException(FftAnalysisPropertyConfig.Mode);

                FftAnalysisReports.Clear();
                foreach (var info in fftAnaylsisResult.perfInfo)
                    FftAnalysisReports.Add(new FftAnalysisReport("性能", info.Key, $"{info.Value.v1:F3} {info.Value.s1}"));
                foreach (var info in fftAnaylsisResult.sgnInfo)
                    FftAnalysisReports.Add(new FftAnalysisReport("信号", info.Key, $"{info.Value.v1:F3} {info.Value.s1}", $"{info.Value.v2:F3} {info.Value.s2}"));

                ChartData.SetSampling(FftAnalysisPropertyConfig.IsSampling || FftAnalysisPropertyConfig.FftN >= FftAnalysisPropertyConfig.AutoSamplingOnDataLength);
                ChartData.ClearData();
                switch ((string)param)
                {
                    case "0":
                        ChartData.AddData(fftAnaylsisResult.t, fftAnaylsisResult.v);
                        break;
                    case "1":
                        ChartData.AddData(fftAnaylsisResult.f, fftAnaylsisResult.p);
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
