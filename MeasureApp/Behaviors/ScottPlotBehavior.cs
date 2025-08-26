using CommunityToolkit.Mvvm.Messaging;
using MeasureApp.Messages;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace MeasureApp.Behaviors
{
    public class ScottPlotBehavior : Behavior<WpfPlot>
        , IRecipient<PlotDataRefreshMessage>
        , IRecipient<PlotResetMessage>
    {
        private readonly DispatcherTimer _renderTimer;
        private string _dataSourceToken;

        public ScottPlotBehavior()
        {
            _renderTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            _renderTimer.Tick += (s, e) =>
            {
                _renderTimer.Stop();
                AssociatedObject?.Refresh();
            };
        }

        #region Plot Dependency Property
        public Plot Plot
        {
            get => (Plot)GetValue(PlotProperty);
            set => SetValue(PlotProperty, value);
        }

        public static readonly DependencyProperty PlotProperty =
            DependencyProperty.Register(nameof(Plot), typeof(Plot), typeof(ScottPlotBehavior),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        #endregion

        #region DataSourceToken Dependency Property
        public string DataSourceToken
        {
            get => (string)GetValue(DataSourceTokenProperty);
            set => SetValue(DataSourceTokenProperty, value);
        }

        public static readonly DependencyProperty DataSourceTokenProperty =
            DependencyProperty.Register(nameof(DataSourceToken), typeof(string), typeof(ScottPlotBehavior),
            new PropertyMetadata(null, OnDataSourceTokenChanged)); // Add a callback

        private static void OnDataSourceTokenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScottPlotBehavior behavior)
            {
                behavior._dataSourceToken = e.NewValue as string;
            }
        }
        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
            {
                this.Plot = AssociatedObject.Plot;
                WeakReferenceMessenger.Default.Register<PlotDataRefreshMessage>(this);
                WeakReferenceMessenger.Default.Register<PlotResetMessage>(this);
            }
        }

        protected override void OnDetaching()
        {
            _renderTimer.Stop();
            WeakReferenceMessenger.Default.UnregisterAll(this);
            this.Plot = null;
            base.OnDetaching();
        }

        /// <summary>
        /// Handles incremental data updates (debounced).
        /// </summary>
        public void Receive(PlotDataRefreshMessage message)
        {
            // Debounce the render call by restarting the timer.
            _renderTimer.Stop();
            _renderTimer.Start();
        }

        /// <summary>
        /// Handles a full plot reset (immediate).
        /// </summary>
        public void Receive(PlotResetMessage message)
        {
            // Stop any pending debounced render.
            _renderTimer.Stop();

            // Perform an immediate refresh on the UI thread.
            AssociatedObject?.Dispatcher.Invoke(() =>
            {
                AssociatedObject?.Refresh();
            });
        }
    }
}
