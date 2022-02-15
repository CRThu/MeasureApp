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

    public class FftAnalysisPropertyConfigs
    {
        [Category("基础设置"), Description("分析模式"), DisplayName("分析模式"), ItemsSource(typeof(FftAnalysisModesSource))]
        public string Mode { get; set; } = "EnergyCorrection";

        [Category("基础设置"), Description("采样率"), DisplayName("采样率")]
        public double Fs { get; set; } = 200000;

        [Category("基础设置"), Description("采样点数(需为2^N)"), DisplayName("采样点数")]
        public int FftN { get; set; } = 32768;

        [Category("基础设置"), Description("窗函数"), DisplayName("窗函数"), ItemsSource(typeof(FftWindowItemsSource))]
        public string Window { get; set; } = "HFT144D";

        [Category("基础设置"), Description("谐波最大测量阶数"), DisplayName("谐波最大测量阶数")]
        public int NHarmonic { get; set; } = 5;

        [Category("渲染设置"), Description("抽样渲染(提高渲染速度)"), DisplayName("抽样渲染")]
        public bool IsSampling { get; set; } = false;

        [Category("渲染设置"), Description("自动抽样渲染点数(提高渲染速度)"), DisplayName("自动抽样渲染点数")]
        public int AutoSamplingOnDataLength { get; set; } = 65536;

        [Category("能量校正设置"), Description("单边信号能量测量跨度"), DisplayName("单边信号能量测量跨度")]
        public int SpanSignalEnergy { get; set; } = 6;

        [Category("能量校正设置"), Description("单边谐波峰值测量跨度"), DisplayName("单边谐波峰值测量跨度")]
        public int SpanHarmonicPeak { get; set; } = 3;

        [Category("能量校正设置"), Description("单边谐波能量测量跨度"), DisplayName("单边谐波能量测量跨度")]
        public int SpanHarmonicEnergy { get; set; } = 1;
    }
}
