using MeasureApp.Model;
using MeasureApp.Model.Common;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MeasureApp.View.Control
{
    public struct Point
    {
        public readonly double X { get; }
        public readonly double Y { get; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public class EChartsLine : WebView2
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(ObservableCollection<Point>), typeof(EChartsLine), new PropertyMetadata(null, DataChangedCallback));

        public static readonly DependencyProperty IsAutoUpdateProperty = DependencyProperty.Register("IsAutoUpdate", typeof(bool), typeof(EChartsLine), new PropertyMetadata(true));

        public static readonly DependencyProperty MinimumTriggerTimeProperty = DependencyProperty.Register("MinimumTriggerTime", typeof(int), typeof(EChartsLine), new PropertyMetadata(0));

        /// <summary>
        /// Y轴数据值
        /// </summary>
        public ObservableCollection<Point> Data
        {
            set
            {
                SetValue(DataProperty, value);
            }
            get
            {
                return (ObservableCollection<Point>)GetValue(DataProperty);
            }
        }

        /// <summary>
        /// 图标是否自动刷新, 默认值true
        /// </summary>
        public bool IsAutoUpdate
        {
            set
            {
                SetValue(IsAutoUpdateProperty, value);
            }
            get
            {
                return (bool)GetValue(IsAutoUpdateProperty);
            }
        }

        /// <summary>
        /// 图表自动刷新最小触发时间, 默认值1000毫秒
        /// </summary>
        public int MinimumTriggerTime
        {
            set
            {
                SetValue(MinimumTriggerTimeProperty, value);
            }
            get
            {
                return (int)GetValue(MinimumTriggerTimeProperty);
            }
        }

        /// <summary>
        /// 上一次图表数据刷新时间
        /// </summary>
        private DateTime dataStorageChartLastRefreshTime = DateTime.Now;

        public EChartsLineData lineData = new();

        private static void DataChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EChartsLine)d).DataChangedCallback(e.NewValue as ObservableCollection<Point>, e.OldValue as ObservableCollection<Point>);
        }

        private void DataChangedCallback(ObservableCollection<Point> newvalue, ObservableCollection<Point> oldvalue)
        {
            if (oldvalue != null)
                oldvalue.CollectionChanged -= Changed;
            if (newvalue != null)
                newvalue.CollectionChanged += Changed;
            Changed(null, null);
        }

        private void Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (IsAutoUpdate)
                    if ((DateTime.Now - dataStorageChartLastRefreshTime).TotalMilliseconds >= MinimumTriggerTime)
                    {
                        // TODO
                        Debug.WriteLine("Selected Data Changed.");
                        dataStorageChartLastRefreshTime = DateTime.Now;
                        UpdateChart();
                    }
                    else
                    {
                        Debug.WriteLine("Busy, Not Triggered.");
                    }
            });
        }

        public void UpdateChart()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //  开始监视代码运行时间

            if (Data != null)
            {
                lineData.SetData(Data.Select(pt=>pt.X), Data.Select(pt => pt.Y));
                string jsons = lineData.ToJson();

                string js = $"UpdateData({jsons});";

                stopwatch.Stop(); //  停止监视
                Debug.WriteLine($"Serialize:{stopwatch.ElapsedMilliseconds}ms, {js.Length}bytes.");

                if (CoreWebView2 != null)
                {
                    Task<string> execTask = this.CoreWebView2.ExecuteScriptAsync(js);
                    Task.Run(() =>
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start(); //  开始监视代码运行时间

                        execTask.Wait();

                        stopwatch.Stop(); //  停止监视
                        Debug.WriteLine($"ExecuteScriptAsync:{stopwatch.ElapsedMilliseconds}ms.");
                    });
                }
            }
        }

        public EChartsLine() : base()
        {
            //this.Source = new Uri("https://html5test.com");
            this.Source = new Uri(Directory.GetCurrentDirectory() + "/View/Web/Chart1.html");
        }
    }
}
