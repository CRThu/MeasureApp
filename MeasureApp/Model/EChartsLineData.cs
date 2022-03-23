using MeasureApp.Model.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MeasureApp.Model
{
    public class EChartsLineData : NotificationObjectBase, INotifyCollectionChanged
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

        public int Count
        {
            get => Data["x"].Count;
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

        public void SetSampling(bool isLttbOn)
        {
            Series["Sampling"] = isLttbOn ? "lttb" : null;
        }

        public void ClearData()
        {
            Data["x"].Clear();
            Data["y"].Clear();
            FireCollectionResetNotification();
        }

        public void AddData(double x, double y)
        {
            Data["x"].Add(x);
            Data["y"].Add(y);
            FireCollectionResetNotification();
        }

        public void AddData(IEnumerable<double> x, IEnumerable<double> y)
        {
            Data["x"].AddRange(x);
            Data["y"].AddRange(y);
            FireCollectionResetNotification();
        }

        public void SetData(IEnumerable<double> x, IEnumerable<double> y)
        {
            Data["x"].Clear();
            Data["y"].Clear();
            Data["x"].AddRange(x);
            Data["y"].AddRange(y);
            FireCollectionResetNotification();
        }

        public void SetData(IEnumerable<double> y)
        {
            Data["x"].Clear();
            Data["y"].Clear();
            Data["x"].AddRange(Enumerable.Range(1, y.Count()).Select(x => (double)x));
            Data["y"].AddRange(y);
            FireCollectionResetNotification();
        }

        public EChartsLineData(bool showSymbol = false, bool smooth = false, EChartsStepLine stepLine = EChartsStepLine.False, bool sampling = true)
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
                { "Sampling", null },
            };
            SetShowSymbol(showSymbol);
            SetSmooth(smooth);
            SetStepLine(stepLine);
            SetSampling(sampling);

            object lockerX = new();
            object lockerY = new();
            BindingOperations.EnableCollectionSynchronization(Data["x"], lockerX);
            BindingOperations.EnableCollectionSynchronization(Data["y"], lockerY);
        }

        public EChartsLineData(double[] x, double[] y, bool showSymbol = false, bool smooth = false, EChartsStepLine stepLine = EChartsStepLine.False, bool sampling = true) : this(showSymbol, smooth, stepLine, sampling)
        {
            AddData(x, y);
        }

        public string ToJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
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
