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
        private readonly AppContextManager _context;
        public AppContextManager Context => _context;

        public DataProcVM(AppContextManager context)
        {
            Title = "数据处理";
            ContentId = "DataProc";
            _context = context;
        }
    }
}
