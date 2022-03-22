using MeasureApp.Model;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
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
    public class EChartsLine : WebView2
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(EChartsLineData), typeof(EChartsLine), new PropertyMetadata(null, DataChangedCallback));

        public EChartsLineData Data
        {
            set
            {
                SetValue(DataProperty, value);
            }
            get
            {
                return (EChartsLineData)GetValue(DataProperty);
            }
        }

        private static void DataChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EChartsLine)d).DataChangedCallback(e.NewValue as EChartsLineData, e.OldValue as EChartsLineData);
        }

        private void DataChangedCallback(EChartsLineData newvalue, EChartsLineData oldvalue)
        {
            if (oldvalue != null)
                oldvalue.CollectionChanged -= Changed;
            if (newvalue != null)
                newvalue.CollectionChanged += Changed;
            // UpdateChart();
        }

        private void Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateChart();
        }

        private void UpdateChart()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //  开始监视代码运行时间

            string jsons = this.Dispatcher.Invoke(() =>
            {
                return Data.ToJson();
            }, DispatcherPriority.Background);

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

        public EChartsLine() : base()
        {
            //this.Source = new Uri("https://html5test.com");
            this.Source = new Uri(Directory.GetCurrentDirectory() + "/View/Web/Chart1.html");
        }
    }
}
