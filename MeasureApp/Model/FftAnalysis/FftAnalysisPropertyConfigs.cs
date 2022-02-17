using MeasureApp.Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace MeasureApp.Model.FftAnalysis
{

    public class FftAnalysisModesSource : IItemsSource
    {
        public Dictionary<string, string> FftAnalysisMode = new()
        {
            { "AmplitudeCorrection", "幅值校正" },
            { "EnergyCorrection", "能量校正" }
        };

        public ItemCollection GetValues()
        {
            ItemCollection modes = new();
            modes.AddRange(FftAnalysisMode.Select(kvp => new Item() { DisplayName = kvp.Value, Value = kvp.Key }));
            return modes;
        }
    }

    public class FftWindowItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection windows = new();
            windows.AddRange(FftWindow.WindowFunc.Keys.ToArray().Select(k => new Item() { DisplayName = k, Value = k }));
            return windows;
        }
    }

    public class FftAnalysisPropertyConfigs : NotificationObjectBase
    {
        private string mode;
        private double fs;
        private int fftN;
        private string window;
        private int nHarmonic;
        private bool isSampling;
        private int autoSamplingOnDataLength;
        private int spanSignalEnergy;
        private int spanHarmonicPeak;
        private int spanHarmonicEnergy;

        [Category("基础设置"), Description("分析模式"), DisplayName("分析模式"), ItemsSource(typeof(FftAnalysisModesSource))]
        public string Mode
        {
            get => mode;
            set
            {
                mode = value;
                RaisePropertyChanged(() => Mode);
            }
        }

        [Category("基础设置"), Description("采样率"), DisplayName("采样率")]
        public double Fs
        {
            get => fs;
            set
            {
                fs = value;
                RaisePropertyChanged(() => Fs);
            }
        }

        [Category("基础设置"), Description("采样点数(需为2^N)"), DisplayName("采样点数")]
        public int FftN
        {
            get => fftN;
            set
            {
                fftN = value;
                RaisePropertyChanged(() => FftN);
            }
        }

        [Category("基础设置"), Description("窗函数"), DisplayName("窗函数"), ItemsSource(typeof(FftWindowItemsSource))]
        public string Window
        {
            get => window;
            set
            {
                window = value;
                RaisePropertyChanged(() => Window);
            }
        }

        [Category("基础设置"), Description("谐波最大测量阶数"), DisplayName("谐波最大测量阶数")]
        public int NHarmonic
        {
            get => nHarmonic;
            set
            {
                nHarmonic = value;
                RaisePropertyChanged(() => NHarmonic);
            }
        }

        [Category("渲染设置"), Description("抽样渲染(提高渲染速度)"), DisplayName("抽样渲染")]
        public bool IsSampling
        {
            get => isSampling;
            set
            {
                isSampling = value;
                RaisePropertyChanged(() => IsSampling);
            }
        }

        [Category("渲染设置"), Description("自动抽样渲染点数(提高渲染速度)"), DisplayName("自动抽样渲染点数")]
        public int AutoSamplingOnDataLength
        {
            get => autoSamplingOnDataLength;
            set
            {
                autoSamplingOnDataLength = value;
                RaisePropertyChanged(() => AutoSamplingOnDataLength);
            }
        }

        [Category("能量校正设置"), Description("单边信号能量测量跨度"), DisplayName("单边信号能量测量跨度")]
        public int SpanSignalEnergy
        {
            get => spanSignalEnergy;
            set
            {
                spanSignalEnergy = value;
                RaisePropertyChanged(() => SpanSignalEnergy);
            }
        }

        [Category("能量校正设置"), Description("单边谐波峰值测量跨度"), DisplayName("单边谐波峰值测量跨度")]
        public int SpanHarmonicPeak
        {
            get => spanHarmonicPeak;
            set
            {
                spanHarmonicPeak = value;
                RaisePropertyChanged(() => SpanHarmonicPeak);
            }
        }

        [Category("能量校正设置"), Description("单边谐波能量测量跨度"), DisplayName("单边谐波能量测量跨度")]
        public int SpanHarmonicEnergy
        {
            get => spanHarmonicEnergy;
            set
            {
                spanHarmonicEnergy = value;
                RaisePropertyChanged(() => SpanHarmonicEnergy);
            }
        }

        public FftAnalysisPropertyConfigs(string mode = "EnergyCorrection",
            double fs = 200000,
            int fftN = 32768,
            string window = "HFT144D",
            int nHarmonic = 5,
            bool isSampling = false,
            int autoSamplingOnDataLength = 65536,
            int spanSignalEnergy = 6,
            int spanHarmonicPeak = 3,
            int spanHarmonicEnergy = 1)
        {
            Mode = mode;
            Fs = fs;
            FftN = fftN;
            Window = window;
            NHarmonic = nHarmonic;
            IsSampling = isSampling;
            AutoSamplingOnDataLength = autoSamplingOnDataLength;
            SpanSignalEnergy = spanSignalEnergy;
            SpanHarmonicPeak = spanHarmonicPeak;
            SpanHarmonicEnergy = spanHarmonicEnergy;
        }
    }
}