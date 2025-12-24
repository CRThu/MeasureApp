using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MeasureApp.Services
{
    public partial class AppDiagnosticsService : ObservableObject
    {
        [ObservableProperty]
        private double currentFps;

        private int _framesCount = 0;
        private Stopwatch _sw = new Stopwatch();


        public AppDiagnosticsService()
        {
            _sw.Start();
            CompositionTarget.Rendering += OnRendering;
        }

        private void OnRendering(object? sender, EventArgs e)
        {
            _framesCount++;

            if (_sw.ElapsedMilliseconds >= 1000)
            {
                CurrentFps = (_framesCount * 1000.0) / _sw.ElapsedMilliseconds;

                _framesCount = 0;
                _sw.Restart();
            }
        }
    }
}
