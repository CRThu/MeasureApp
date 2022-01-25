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
        private Dictionary<string, string> series;
        public Dictionary<string, string> Series
        {
            get => series;
            set
            {
                series = value;
                RaisePropertyChanged(() => Series);
            }
        }

        public EChartsLineData()
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
                { "DataYColumnsName", "y" }
            };
        }

        public EChartsLineData(double[] x, double[] y) : this()
        {
            Data["x"] = new ObservableRangeCollection<double>(x);
            Data["y"] = new ObservableRangeCollection<double>(y);
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
