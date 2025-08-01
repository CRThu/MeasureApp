using CarrotLink.Core.Session;
using MeasureApp.Services;
using ScottPlot.TickGenerators.Financial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeasureApp.Model
{
    public class ScriptExec
    {
        public string PreferredDevice { get; set; }

        public int Interval { get; set; }

        public string[] Codes { get; private set; }

        public int CurrentLine { get; set; }

        private readonly AppContextManager _context;

        private Task _exec;
        private CancellationTokenSource _cts;

        public ScriptExec(AppContextManager context)
        {
            _context = context;
        }

        public void Start(string code, int interval = 100, int startLine = 0)
        {
            Codes = code.Split('\n');
            Interval = interval;
            CurrentLine = startLine;

            _cts = new CancellationTokenSource();
            _exec = ExecTask();
        }

        public void Stop()
        {
            _cts.Cancel();
        }

        public async Task ExecTask()
        {
            try
            {
                while (CurrentLine < Codes.Length)
                {
                    if (_cts.IsCancellationRequested)
                        return;

                    RunOne(Codes[CurrentLine]);
                    CurrentLine++;

                    await Task.Delay(Interval, _cts.Token);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void RunOne(string code)
        {
            _context.Devices[PreferredDevice].SendAscii(code + "\n");
        }
    }
}
