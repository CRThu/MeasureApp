using CarrotLink.Core.Session;
using CommunityToolkit.Mvvm.ComponentModel;
using MeasureApp.Services;
using Newtonsoft.Json.Linq;
using ScottPlot.TickGenerators.Financial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeasureApp.Services.Script
{
    public partial class ScriptExec : ObservableObject
    {
        public string PreferredDevice { get; set; }

        [ObservableProperty]
        private int interval = 500;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ScriptLines))]
        private string scriptCode;

        public string[] ScriptLines => ScriptCode?.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

        [ObservableProperty]
        private int currentLine = 1;

        [ObservableProperty]
        private bool isRunning;

        private Task _exec;
        private CancellationTokenSource _cts;

        private readonly AppContextManager _context;

        public ScriptExec(AppContextManager context)
        {
            _context = context;
        }

        public void Step()
        {
            if (!IsRunning)
            {
                RunOneLine().GetAwaiter().GetResult();

                if (CurrentLine > ScriptLines?.Length)
                    CurrentLine = 1;
            }
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _exec = ExecTask();
        }

        public void Stop()
        {
            _cts.Cancel();
        }

        public void Reset()
        {
            CurrentLine = 1;
        }

        private void SkipEmptyLine()
        {
            while (CurrentLine <= ScriptLines?.Length
                && string.IsNullOrWhiteSpace(ScriptLines[CurrentLine - 1]))
                CurrentLine++;
        }

        private async Task RunOneLine()
        {
            // 跳过前空行
            SkipEmptyLine();

            // 运行当前行
            if (CurrentLine <= ScriptLines?.Length)
                await Emit(ScriptLines[CurrentLine - 1]);

            CurrentLine++;

            // 跳过后空行
            SkipEmptyLine();
        }

        public async Task ExecTask()
        {
            try
            {
                IsRunning = true;
                while (CurrentLine <= ScriptLines?.Length)
                {
                    if (_cts.IsCancellationRequested)
                        return;

                    await RunOneLine();

                    // 最后一行不需要delay
                    if (CurrentLine > ScriptLines?.Length)
                        break;

                    // 延迟
                    await Task.Delay(Interval, _cts.Token);
                }

                // 运行结束置起始位
                CurrentLine = 1;
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                _cts.Cancel();
                IsRunning = false;
            }
        }

        private async Task Emit(string code)
        {
            await _context.Devices[PreferredDevice].SendAscii(code + "\n");
        }
    }
}
