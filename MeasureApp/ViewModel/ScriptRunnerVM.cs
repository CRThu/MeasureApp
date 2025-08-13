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
        private ScriptExecution exec;

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
            Exec = new ScriptExecution(_context);
        }

        partial void OnSelectedDeviceChanged(ConnectionInfo value)
        {
            Exec.Environment.Set(Exec.EnvDefaultIOName, value?.InternalKey);
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
            try
            {
                if (!Exec.IsRunning)
                {
                    Exec.Start(ScriptExecutionMode.Continuous);
                }
                else
                {
                    Exec.Stop();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void ScriptRunStep()
        {
            try
            {
                if (!Exec.IsRunning)
                {
                    Exec.Start(ScriptExecutionMode.Step);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void ScriptReset()
        {
            try
            {
                if (!Exec.IsRunning)
                {
                    Exec.Reset();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}