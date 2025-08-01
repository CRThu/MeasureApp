using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MeasureApp.Model;
using MeasureApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeasureApp.ViewModel
{
    public partial class ScriptRunnerVM : BaseVM
    {
        [ObservableProperty]
        private ObservableCollection<ScriptViewer> scriptViewers = new()
        {
            new ScriptViewer() { Header = "Tab1", Text = "Text1" },
            new ScriptViewer() { Header = "Tab2", Text = "Text2" }
        };

        [RelayCommand]
        public void Closed()
        {
            MessageBox.Show("Closed Called");
        }
    }
}
