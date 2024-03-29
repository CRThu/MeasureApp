﻿using MeasureApp.Model.Common;
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
        private int fromBase;
        private int bits;
        private double vBias;
        private bool hasCodeOffset;
        private bool isAveragedFft;
        private int averagedFftLength;
        private double averagedFftOverlap;
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
        private double fullScale;
        private bool isNoiseCorrection;
        private bool isTestCosineWave;
        private double testCosineWaveFreq;
        private double? testCosineWaveSnr;

        [Category("样本设置"), Description("数据进制"), DisplayName("数据进制")]
        public int FromBase
        {
            get => fromBase;
            set
            {
                fromBase = value;
                RaisePropertyChanged(() => FromBase);
            }
        }

        [Category("样本设置"), Description("字长(Bits)"), DisplayName("字长(Bits)")]
        public int Bits
        {
            get => bits;
            set
            {
                bits = value;
                RaisePropertyChanged(() => Bits);
            }
        }

        [Category("样本设置"), Description("偏置电压"), DisplayName("偏置电压")]
        public double VBias
        {
            get => vBias;
            set
            {
                vBias = value;
                RaisePropertyChanged(() => VBias);
            }
        }

        [Category("样本设置"), Description("偏移零为中间码字"), DisplayName("偏移零为中间码字")]
        public bool HasCodeOffset
        {
            get => hasCodeOffset;
            set
            {
                hasCodeOffset = value;
                RaisePropertyChanged(() => HasCodeOffset);
            }
        }

        [Category("切片设置"), Description("使用平均FFT(Alpha)"), DisplayName("使用平均FFT(Alpha)")]
        public bool IsAveragedFft
        {
            get => isAveragedFft;
            set
            {
                isAveragedFft = value;
                RaisePropertyChanged(() => IsAveragedFft);
            }
        }

        [Category("切片设置"), Description("平均FFT点数"), DisplayName("平均FFT点数")]
        public int AveragedFftLength
        {
            get => averagedFftLength;
            set
            {
                averagedFftLength = value;
                RaisePropertyChanged(() => AveragedFftLength);
            }
        }

        [Category("切片设置"), Description("平均FFT重叠率"), DisplayName("平均FFT重叠率")]
        public double AveragedFftOverlap
        {
            get => averagedFftOverlap;
            set
            {
                averagedFftOverlap = value;
                RaisePropertyChanged(() => AveragedFftOverlap);
            }
        }

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

        [Category("基础设置"), Description("满幅电平(正负轨电压差)"), DisplayName("满幅电平(正负轨电压差)")]
        public double FullScale
        {
            get => fullScale;
            set
            {
                fullScale = value;
                RaisePropertyChanged(() => FullScale);
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

        [Category("信号设置"), Description("单边信号能量测量跨度"), DisplayName("单边信号能量测量跨度")]
        public int SpanSignalEnergy
        {
            get => spanSignalEnergy;
            set
            {
                spanSignalEnergy = value;
                RaisePropertyChanged(() => SpanSignalEnergy);
            }
        }

        [Category("信号设置"), Description("单边谐波峰值测量跨度"), DisplayName("单边谐波峰值测量跨度")]
        public int SpanHarmonicPeak
        {
            get => spanHarmonicPeak;
            set
            {
                spanHarmonicPeak = value;
                RaisePropertyChanged(() => SpanHarmonicPeak);
            }
        }

        [Category("信号设置"), Description("单边谐波能量测量跨度"), DisplayName("单边谐波能量测量跨度")]
        public int SpanHarmonicEnergy
        {
            get => spanHarmonicEnergy;
            set
            {
                spanHarmonicEnergy = value;
                RaisePropertyChanged(() => SpanHarmonicEnergy);
            }
        }

        [Category("噪声设置"), Description("噪声补偿"), DisplayName("噪声补偿")]
        public bool IsNoiseCorrection
        {
            get => isNoiseCorrection;
            set
            {
                isNoiseCorrection = value;
                RaisePropertyChanged(() => IsNoiseCorrection);
            }
        }

        [Category("基准测试"), Description("基准波形自测试"), DisplayName("基准波形自测试")]
        public bool IsTestCosineWave
        {
            get => isTestCosineWave;
            set
            {
                isTestCosineWave = value;
                RaisePropertyChanged(() => IsTestCosineWave);
            }
        }

        [Category("基准测试"), Description("基准波形频率"), DisplayName("基准波形频率")]
        public double TestCosineWaveFreq
        {
            get => testCosineWaveFreq;
            set
            {
                testCosineWaveFreq = value;
                RaisePropertyChanged(() => TestCosineWaveFreq);
            }
        }

        [Category("基准测试"), Description("基准波形信噪比"), DisplayName("基准波形信噪比")]
        public double? TestCosineWaveSnr
        {
            get => testCosineWaveSnr;
            set
            {
                testCosineWaveSnr = value;
                RaisePropertyChanged(() => TestCosineWaveSnr);
            }
        }

        public FftAnalysisPropertyConfigs(
            int fromBase = 16,
            int bits = 16,
            double vBias = 0,
            bool hasCodeOffset = true,
            bool isAveragedFft = false,
            int averagedFftLength = 16384,
            double averagedFftOverlap = 0.9,
            string mode = "EnergyCorrection",
            double fs = 200000,
            int fftN = 32768,
            string window = "HFT144D",
            int nHarmonic = 5,
            bool isSampling = false,
            int autoSamplingOnDataLength = 65536,
            int spanSignalEnergy = 6,
            int spanHarmonicPeak = 3,
            int spanHarmonicEnergy = 6,
            double fullScale = 10,
            bool isNoiseCorrection = false,
            bool isTestCosineWave = false,
            double testCosineWaveFreq = 1000,
            double? testCosineWaveSnr = null)
        {
            FromBase = fromBase;
            Bits = bits;
            VBias = vBias;
            HasCodeOffset = hasCodeOffset;
            IsAveragedFft = isAveragedFft;
            AveragedFftLength = averagedFftLength;
            AveragedFftOverlap = averagedFftOverlap;
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
            FullScale = fullScale;
            IsNoiseCorrection = isNoiseCorrection;
            IsTestCosineWave = isTestCosineWave;
            TestCosineWaveFreq = testCosineWaveFreq;
            TestCosineWaveSnr = testCosineWaveSnr;
        }
    }
}