using MeasureApp.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MeasureApp.Model
{
    public class EChartsLineData : NotificationObjectBase
    {
        // 图表标题
        private string title;
        public string Title
        {
            get => title;
            set
            {
                title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        // 图表数据
        private Dictionary<string, ObservableRangeCollection<double>> data;
        public Dictionary<string, ObservableRangeCollection<double>> Data
        {
            get => data;
            set
            {
                data = value;
                RaisePropertyChanged(() => Data);
            }
        }

        // 图表配置
        private Dictionary<string, object> series;
        public Dictionary<string, object> Series
        {
            get => series;
            set
            {
                series = value;
                RaisePropertyChanged(() => Series);
            }
        }

        public void SetShowSymbol(bool showSymbol)
        {
            Series["ShowSymbol"] = showSymbol;
        }

        public void SetSmooth(bool smooth)
        {
            Series["Smooth"] = smooth;
        }

        public void SetStepLine(bool stepLine)
        {
            if (!stepLine)
                SetStepLine(EChartsStepLine.False);
            else
                SetStepLine(EChartsStepLine.Middle);
        }

        public void SetStepLine(EChartsStepLine stepLine)
        {
            object option;
            switch (stepLine)
            {
                case EChartsStepLine.False:
                    option = false;
                    break;
                case EChartsStepLine.Start:
                    option = "start";
                    break;
                case EChartsStepLine.Middle:
                    option = "middle";
                    break;
                case EChartsStepLine.End:
                    option = "end";
                    break;
                default:
                    option = false;
                    break;
            }
            Series["Step"] = option;
        }

        public EChartsLineData(bool showSymbol = false, bool smooth = false, EChartsStepLine stepLine = EChartsStepLine.False)
        {
            Data = new()
            {
                { "x", new ObservableRangeCollection<double>() },
                { "y", new ObservableRangeCollection<double>() }
            };
            Series = new()
            {
                { "Name", "Data" },
                { "Type", "line" },
                { "DataXColumnsName", "x" },
                { "DataYColumnsName", "y" },
                { "ShowSymbol", null },
                { "Smooth", null },
                { "Step", null },
            };
            SetShowSymbol(showSymbol);
            SetSmooth(smooth);
            SetStepLine(stepLine);
        }

        public EChartsLineData(double[] x, double[] y, bool showSymbol = false, bool smooth = false, EChartsStepLine stepLine = EChartsStepLine.False) : this(showSymbol, smooth, stepLine)
        {
            Data["x"] = new ObservableRangeCollection<double>(x);
            Data["y"] = new ObservableRangeCollection<double>(y);
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public enum EChartsStepLine
    {
        False = 0,
        Start = 1,
        Middle = 2,
        End = 3
    }
}
