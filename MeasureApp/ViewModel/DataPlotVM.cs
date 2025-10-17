using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DryIoc;
using MeasureApp.Messages;
using MeasureApp.Model.Common;
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
                throw new NotImplementedException();
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
                List<double> _plotYData = dataYSnapshot
                    .Select(v => v.Type == DataLogValue.ValueType.Double ? v.Double : Convert.ToDouble(v.Int64))
                    .ToList();

                IHasLegendText sig;
                if (IsCustomDataXUsed
                    && Context.DataLogger.TryGetValue(SelectedDataXKey, out DataLogList dataXLog))
                {
                    var dataXSnapshot = dataXLog.GetSnapshot();
                    List<double> _plotXData = dataXSnapshot
                        .Select(v => v.Type == DataLogValue.ValueType.Double ? v.Double : Convert.ToDouble(v.Int64))
                        .ToList();

                    // sort性能优化
                    var sortedData = _plotXData.Zip(_plotYData, (x, y) => new { X = x, Y = y })
                        .OrderBy(item => item.X)
                        .ToList();

                    sig = Plot.Add.SignalXY(sortedData.Select(s=>s.X).ToArray()
                        , sortedData.Select(s => s.Y).ToArray());

                    //sig = Plot.Add.Signal(_plotXData, _plotYData);
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
        public void AddTestData()
        {
            try
            {
                Context.DataLogger.Clear();
                Context.DataLogger.AddRange("X1", new int[] { 1, 2, 4, 9, 16, 25, 36, 49, 64, 81 });
                Context.DataLogger.AddRange("X2", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
                Context.DataLogger.AddRange("Y1", new int[] { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 });
                Context.DataLogger.AddRange("Y2", new double[] { 1, 2, 4, 9, 16, 25, 36, 49, 64, 81 });
                Random random = new Random();
                for (int i = 0; i < 10; i++)
                    Context.DataLogger.Add("Y3", random.Next());
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }
    }
}
