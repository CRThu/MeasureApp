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
    public partial class DataProcVM : BaseVM
    {
        private readonly DataLogService _dataLogger;
        public DataLogService DataLogger => _dataLogger;

        public DataProcVM(DataLogService dataLogger)
        {
            Title = "数据处理";
            ContentId = "DataProc";
            _dataLogger = dataLogger;
        }
    }
}
