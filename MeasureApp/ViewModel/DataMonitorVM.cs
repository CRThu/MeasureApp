using CarrotLink.Core.Session;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DryIoc;
using MeasureApp.Messages;
using MeasureApp.Services;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.ViewModel
{
    public partial class DataMonitorVM : BaseVM
    {
        private readonly AppContextManager _context;
        public AppContextManager Context => _context;

        [ObservableProperty]
        private string selectedKey;

        [ObservableProperty]
        private string addKeyText;

        private DataLogList _selectedData;
        public DataLogList SelectedData => _selectedData;

        [ObservableProperty]
        private Plot plot;

        private List<double> _plotData = new();
        private int _lastProcessedCount = 0;
        private readonly object _dataLock = new object();

        public DataMonitorVM(AppContextManager context)
        {
            _context = context;
        }

        partial void OnPlotChanged(Plot value)
        {
            if (SelectedKey != null)
            {
                InitializePlot();
            }
        }

        partial void OnSelectedKeyChanged(string oldValue, string newValue)
        {
            // Unsubscribe from old data source's events
            if (_selectedData != null)
            {
                ((INotifyPropertyChanged)_selectedData).PropertyChanged -= OnSelectedDataPropertyChanged;
            }

            // Update the selected data source
            _selectedData = GetSelectedData();
            OnPropertyChanged(nameof(SelectedData));

            // Subscribe to new data source's events
            if (_selectedData != null)
            {
                ((INotifyPropertyChanged)_selectedData).PropertyChanged += OnSelectedDataPropertyChanged;
            }

            // Re-initialize the plot for the new data source
            InitializePlot();
        }

        private DataLogList GetSelectedData()
        {
            if (SelectedKey != null && Context.DataLogger.Contains(SelectedKey))
            {
                return Context.DataLogger[SelectedKey];
            }
            return null;
        }

        private void InitializePlot()
        {
            if (Plot is null)
                return;

            lock (_dataLock)
            {
                Plot.Clear();
                _lastProcessedCount = 0;

                if (SelectedData != null)
                {
                    // Initial data load into the List<double>
                    // 初始数据加载到 List<double>
                    var dataSnapshot = SelectedData.GetSnapshot();
                    _plotData = dataSnapshot
                        .Select(v => v.Type == DataLogValue.ValueType.Double ? v.Double : Convert.ToDouble(v.Int64))
                        .ToList();
                    _lastProcessedCount = _plotData.Count;

                    var signalPlot = Plot.Add.Signal(_plotData);
                    signalPlot.LegendText = SelectedKey;

                    Plot.Axes.Title.Label.Text = $"Data for '{SelectedKey}'";
                    Plot.Axes.Bottom.Label.Text = "Index";
                    Plot.Axes.Left.Label.Text = "Value";
                    Plot.Legend.IsVisible = true;

                    Plot.Axes.AntiAlias(true);
                }
                else
                {
                    _plotData.Clear();
                    Plot.Axes.Title.Label.Text = "No Data Selected";
                }

                Plot.Axes.AutoScale();
            }

            if (!string.IsNullOrEmpty(SelectedKey))
            {
                WeakReferenceMessenger.Default.Send(PlotResetMessage.Instance);
            }
        }

        private void OnSelectedDataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DataLogList.Items))
            {
                Task.Run(() => UpdatePlotDataAndNotify());
            }
        }

        private void UpdatePlotDataAndNotify()
        {
            if (SelectedData is null || Plot is null)
                return;

            var dataSnapshot = SelectedData.GetSnapshot();
            bool dataWasUpdated = false;

            lock (_dataLock)
            {
                int currentSourceCount = dataSnapshot.Length;
                int newItemsCount = currentSourceCount - _lastProcessedCount;

                if (newItemsCount <= 0)
                    return;

                dataWasUpdated = true;

                var newPoints = dataSnapshot
                    .Skip(_lastProcessedCount)
                    .Select(v => v.Type == DataLogValue.ValueType.Double ? v.Double : Convert.ToDouble(v.Int64));

                _plotData.AddRange(newPoints);
                _lastProcessedCount = currentSourceCount;
            }

            if (dataWasUpdated)
            {
                // This message is for debounced, incremental updates.
                // 此消息用于防抖的增量更新。
                WeakReferenceMessenger.Default.Send(PlotDataRefreshMessage.Instance);
            }
        }

        public void Dispose()
        {
            if (_selectedData != null)
            {
                ((INotifyPropertyChanged)_selectedData).PropertyChanged -= OnSelectedDataPropertyChanged;
            }
        }

        [RelayCommand]
        public void AddKey()
        {
            try
            {
                Context.DataLogger.GetOrAddKey(AddKeyText);
                if (SelectedKey == null)
                    SelectedKey = Context.DataLogger.Keys.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void RemoveSelectedKey()
        {
            try
            {
                if (SelectedKey != null && Context.DataLogger.Contains(SelectedKey))
                {
                    Context.DataLogger.RemoveKey(SelectedKey);
                }
                SelectedKey = Context.DataLogger.Keys.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void RemoveAllKeys()
        {
            try
            {
                Context.DataLogger.Clear();
                SelectedKey = null;
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void CopySelectedKeyToClipboard()
        {
            try
            {
                if (SelectedKey != null && Context.DataLogger.Contains(SelectedKey))
                {
                    Context.DataLogger.CopyToClipboard(SelectedKey);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void DataTest(object param)
        {
            try
            {
                Random random = new Random();
                string paramString = Convert.ToString(param);
                switch (paramString)
                {
                    case "D+1":
                        Task.Run(() =>
                        {
                            if (SelectedKey != null)
                            {
                                Context.DataLogger.Add(SelectedKey, random.NextDouble());
                            }
                        });
                        break;
                    case "D+1M":
                        for (int i = 0; i < 100; i++)
                        {
                            Task.Run(() =>
                            {
                                if (SelectedKey != null)
                                {
                                    Int64[] ints = new Int64[10000];
                                    for (int i = 0; i < ints.Length; i++)
                                        ints[i] = random.NextInt64();
                                    Context.DataLogger.AddRange(SelectedKey, ints);
                                }
                            });
                        }
                        break;
                    case "K+1":
                        Task.Run(() =>
                        {
                            Context.DataLogger.GetOrAddKey(random.NextInt64().ToString());
                        });
                        break;
                    case "K+1K":
                        for (int i = 0; i < 10; i++)
                        {
                            Task.Run(() =>
                            {
                                for (int i = 0; i < 100; i++)
                                {
                                    Context.DataLogger.GetOrAddKey(random.NextInt64().ToString());
                                }
                            });
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }
    }
}
