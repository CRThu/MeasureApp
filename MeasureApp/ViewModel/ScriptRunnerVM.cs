using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MeasureApp.Services;
using MeasureApp.Services.Script;
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

        [ObservableProperty]
        private ScriptExec exec;

        //[ObservableProperty]
        //private ObservableCollection<ScriptViewer> scriptViewers = new()
        //{
        //    new ScriptViewer() { Header = "Tab1", Text = "Text1" },
        //    new ScriptViewer() { Header = "Tab2", Text = "Text2" }
        //};

        [ObservableProperty]
        private string scriptPath;

        public ScriptRunnerVM(AppContextManager context)
        {
            _context = context;
            Exec = new ScriptExec(_context);
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
                Exec.ScriptCode = File.ReadAllText(ScriptPath);
            }
        }

        [RelayCommand]
        public void ScriptRun()
        {
            if (!Exec.IsRunning)
            {
                if (SelectedDevice == null)
                {
                    MessageBox.Show("需要选择设备");
                    return;
                }
                Exec.PreferredDevice = SelectedDevice.InternalKey;
                Exec.Start();
            }
            else
            {
                Exec.Stop();
            }
        }

        [RelayCommand]
        public void ScriptRunStep()
        {
            if (!Exec.IsRunning)
            {
                if (SelectedDevice == null)
                {
                    MessageBox.Show("需要选择设备");
                    return;
                }
                Exec.PreferredDevice = SelectedDevice.InternalKey;

                Exec.Step();
            }
        }

        [RelayCommand]
        public void ScriptReset()
        {
            if (!Exec.IsRunning)
            {
                Exec.Reset();
            }
        }
    }
}