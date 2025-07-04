﻿using DryIoc;
using MeasureApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.ViewModel
{
    public class ViewModelLocator
    {
        private readonly Container container;

        public ViewModelLocator()
        {
            container = new Container();

            // Register
            container.Register<MainWindowVM>();
            container.Register<DeviceConnectionVM>();
        }

        public MainWindowVM MainWindow => container.Resolve<MainWindowVM>();

        public DeviceConnectionVM DeviceConnection => container.Resolve<DeviceConnectionVM>();
    }
}
