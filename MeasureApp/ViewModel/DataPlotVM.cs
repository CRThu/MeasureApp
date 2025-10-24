using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DryIoc;
using MeasureApp.Messages;
using MeasureApp.Services;
using ScottPlot;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace MeasureApp.ViewModel
{
    public partial class DataPlotVM : BaseVM
    {
        private readonly AppContextManager _context;
        public AppContextManager Context => _context;

        [ObservableProperty]
        private string selectedDataXKey;

        [ObservableProperty]
        private string selectedDataYKey;

        [ObservableProperty]
        private bool isCustomDataXUsed = false;

        [ObservableProperty]
        private bool isAutoUpdateEnabled = true;

        [ObservableProperty]
        private Plot plot;

        List<double> _plotXData;
        List<double> _plotYData;

        int testDataCnt1 = 0;
        int testDataCnt2 = 0;


        public DataPlotVM(AppContextManager context)
        {
            _context = context;
        }

        partial void OnIsCustomDataXUsedChanged(bool value)
        {
            if (value)
            {
                if (Context.DataLogger.TryGetValue(SelectedDataXKey, out var xDataLog))
                {
                    xDataLog.PropertyChanged += (_, _) => PlotDataUpdate();
                }
            }
            else
            {
                if (Context.DataLogger.TryGetValue(SelectedDataXKey, out var xDataLog))
                {
                    xDataLog.PropertyChanged -= (_, _) => PlotDataUpdate();
                }
            }
        }

        partial void OnSelectedDataXKeyChanged(string oldValue, string newValue)
        {
            if (Context.DataLogger.TryGetValue(oldValue, out var oldDataLog))
            {
                oldDataLog.PropertyChanged -= (_, _) => PlotDataUpdate();
            }
            if (IsCustomDataXUsed)
            {
                if (Context.DataLogger.TryGetValue(newValue, out var newDataLog))
                {
                    newDataLog.PropertyChanged += (_, _) => PlotDataUpdate();
                }
            }
            PlotSourceUpdate();
        }

        partial void OnSelectedDataYKeyChanged(string oldValue, string newValue)
        {
            if (Context.DataLogger.TryGetValue(oldValue, out var oldDataLog))
            {
                oldDataLog.PropertyChanged -= (_, _) => PlotDataUpdate();
            }
            if (Context.DataLogger.TryGetValue(newValue, out var newDataLog))
            {
                newDataLog.PropertyChanged += (_, _) => PlotDataUpdate();
            }
            PlotSourceUpdate();
        }

        private void PlotDataUpdate()
        {
            try
            {
                bool dataWasUpdated = false;
                if (_plotYData != null)
                {
                    if (!Context.DataLogger.TryGetValue(SelectedDataYKey, out DataLogList dataYLog))
                        return;

                    var dataYSnapshot = dataYLog.GetSnapshot();
                    int newItemsCount = dataYSnapshot.Length - _plotYData.Count;

                    if (newItemsCount <= 0)
                        return;

                    dataWasUpdated = true;
                    var newPoints = dataYSnapshot
                        .Skip(_plotYData.Count)
                        .Select(v => v.Type == DataLogValue.ValueType.Double ? v.Double : Convert.ToDouble(v.Int64));
                    _plotYData.AddRange(newPoints);
                }
                if (IsCustomDataXUsed && _plotXData != null)
                {
                    if (!Context.DataLogger.TryGetValue(SelectedDataXKey, out DataLogList dataXLog))
                        return;

                    var dataXSnapshot = dataXLog.GetSnapshot();
                    int newItemsCount = dataXSnapshot.Length - _plotXData.Count;

                    if (newItemsCount <= 0)
                        return;

                    dataWasUpdated = true;
                    var newPoints = dataXSnapshot
                        .Skip(_plotXData.Count)
                        .Select(v => v.Type == DataLogValue.ValueType.Double ? v.Double : Convert.ToDouble(v.Int64));
                    _plotXData.AddRange(newPoints);
                }
                if (dataWasUpdated)
                {
                    // 此消息用于防抖的增量更新。
                    WeakReferenceMessenger.Default.Send(PlotDataRefreshMessage.Instance);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        private void PlotSourceUpdate()
        {
            try
            {
                if (Plot == null
                || SelectedDataYKey == null)
                    return;

                Plot.Clear();
                if (!Context.DataLogger.TryGetValue(SelectedDataYKey, out DataLogList dataYLog))
                    return;

                var dataYSnapshot = dataYLog.GetSnapshot();
                _plotYData = dataYSnapshot
                    .Select(v => v.Type == DataLogValue.ValueType.Double ? v.Double : Convert.ToDouble(v.Int64))
                    .ToList();

                IHasLegendText sig;
                if (IsCustomDataXUsed
                    && Context.DataLogger.TryGetValue(SelectedDataXKey, out DataLogList dataXLog))
                {
                    var dataXSnapshot = dataXLog.GetSnapshot();
                    _plotXData = dataXSnapshot
                        .Select(v => v.Type == DataLogValue.ValueType.Double ? v.Double : Convert.ToDouble(v.Int64))
                        .ToList();

                    // sort性能优化
                    //var sortedData = _plotXData.Zip(_plotYData, (x, y) => new { X = x, Y = y })
                    //    .OrderBy(item => item.X)
                    //    .ToList();

                    //sig = Plot.Add.SignalXY(sortedData.Select(s=>s.X).ToArray()
                    //    , sortedData.Select(s => s.Y).ToArray());

                    sig = Plot.Add.ScatterLine(_plotXData, _plotYData);
                }
                else
                {
                    sig = Plot.Add.Signal(_plotYData);
                }
                sig.LegendText = SelectedDataYKey;
                Plot.Axes.Title.Label.Text = $"Data for '{SelectedDataYKey}'";
                Plot.Axes.Bottom.Label.Text = "Index";
                Plot.Axes.Left.Label.Text = "Value";
                Plot.Legend.IsVisible = true;

                Plot.Axes.AntiAlias(true);

                Plot.Axes.AutoScale();

                WeakReferenceMessenger.Default.Send(PlotResetMessage.Instance);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void ManualRefresh()
        {
            try
            {
                PlotSourceUpdate();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void GenTestData()
        {
            try
            {
                Random random = new Random();

                Context.DataLogger.Clear();
                Context.DataLogger.AddRange("X1", new double[] { 1, 4, 9, 16, 25, 36, 49, 64, 81, 100 });
                Context.DataLogger.AddRange("X2", new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
                Context.DataLogger.AddRange("Y1", new double[] { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 });
                Context.DataLogger.AddRange("Y2", new double[] { 1, 4, 9, 16, 25, 36, 49, 64, 81, 100 });
                Context.DataLogger.AddRange("Y3", Enumerable.Range(1, 10).Select((_) => random.NextDouble()));
                Context.DataLogger.AddRange("XL1", Enumerable.Range(1, 100000));
                Context.DataLogger.AddRange("YL1", Enumerable.Range(1, 100000));
                Context.DataLogger.AddRange("YL2", Enumerable.Range(1, 100000).Select((_) => random.NextDouble()));
                testDataCnt1 = 10;
                testDataCnt2 = 100000;
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }


        [RelayCommand]
        public void AddTestData()
        {
            try
            {
                testDataCnt1++;
                testDataCnt2++;
                Random random = new Random();
                Context.DataLogger.Add("X1", testDataCnt1 * testDataCnt1);
                Context.DataLogger.Add("X2", testDataCnt1);
                Context.DataLogger.Add("Y1", testDataCnt1 % 2 == 0 ? 1 : 0);
                Context.DataLogger.Add("Y2", testDataCnt1 * testDataCnt1);
                Context.DataLogger.Add("Y3", random.NextDouble());
                Context.DataLogger.Add("XL1", testDataCnt2);
                Context.DataLogger.Add("YL1", testDataCnt2);
                Context.DataLogger.Add("YL2", random.NextDouble());

            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }
    }
}