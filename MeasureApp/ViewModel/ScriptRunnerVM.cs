using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MeasureApp.Model;
using MeasureApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeasureApp.ViewModel
{
    public partial class ScriptRunnerVM : BaseVM
    {
        [ObservableProperty]
        private ConnectionInfo selectedDevice;

        private readonly AppContextManager _context;
        public AppContextManager Context => _context;

        private readonly ScriptExec _scriptExec;

        //[ObservableProperty]
        //private ObservableCollection<ScriptViewer> scriptViewers = new()
        //{
        //    new ScriptViewer() { Header = "Tab1", Text = "Text1" },
        //    new ScriptViewer() { Header = "Tab2", Text = "Text2" }
        //};

        [ObservableProperty]
        private int scriptRunInterval = 500;

        [ObservableProperty]
        private bool isScriptRunnning = false;

        [ObservableProperty]
        private string scriptPath;

        [ObservableProperty]
        private string scriptCode;

        //[RelayCommand]
        //public void Closed(object v)
        //{
        //    MessageBox.Show($"Closed Called: {v}");
        //}
        public ScriptRunnerVM(AppContextManager context)
        {
            _context = context;
            _scriptExec = new ScriptExec(_context);
        }

        [RelayCommand]
        public void OpenScriptFile()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                InitialDirectory = Environment.CurrentDirectory,
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ScriptPath = ofd.FileName;
                ScriptCode = File.ReadAllText(ScriptPath);
            }
        }

        [RelayCommand]
        public void ScriptRun()
        {
            if (!IsScriptRunnning)
            {
                if (SelectedDevice == null)
                {
                    MessageBox.Show("需要选择设备");
                    return;
                }
                _scriptExec.PreferredDevice = SelectedDevice.InternalKey;
                _scriptExec.Start(ScriptCode, ScriptRunInterval);
                IsScriptRunnning = true;
            }
            else
            {
                _scriptExec.Stop();
                IsScriptRunnning = false;
            }
        }
    }
}
