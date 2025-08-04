using CarrotLink.Core.Session;
using CommunityToolkit.Mvvm.ComponentModel;
using MeasureApp.Model.Script;
using MeasureApp.Model.SerialPortScript;
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
        public readonly string EnvDefaultIOName = "Env::Default::IO";

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

        private readonly Dictionary<string, string> _env;
        private readonly object _envLock = new object();

        public ScriptExec(AppContextManager context)
        {
            _context = context;
            _env = new Dictionary<string, string>();
        }

        public void SetEnv(string key, string val)
        {
            lock (_envLock)
            {
                if (!_env.TryAdd(key, val))
                    _env[key] = val;
            }
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
            lock (_envLock)
            {
                _env.Clear();
            }
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
            {
                // 若存在注释则滤除注释
                string scriptLine = ScriptLines[CurrentLine - 1];
                if (ScriptLines[CurrentLine - 1].Contains('#'))
                    scriptLine = scriptLine.Split('#', 2).First().Trim();

                if (scriptLine.Length > 0)
                    await Emit(scriptLine);
            }

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
            if (XmlTag.IsMatchXmlTag(code))
            {
                // 调用脚本库方法
                // TODO 重写
                ScriptMethodParameters parameters = new ScriptMethodParameters(
                    XmlTag.GetXmlTagAttrs(code));

                string methodName = parameters.Get<string>("Tag");

                switch (methodName.ToUpper())
                {
                    case "ENV":
                        string k = parameters.Get<string>("key") ?? "<null>";
                        string v = parameters.Get<string>("value") ?? "<null>";
                        SetEnv(k, v);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // default operation
                string defaultIOKey;
                lock (_envLock)
                {
                    _env.TryGetValue(EnvDefaultIOName, out defaultIOKey);
                }
                if (defaultIOKey != null)
                {
                    await _context.Devices[defaultIOKey].SendAscii(code + "\n");
                }
                else
                {
                    throw new InvalidOperationException("No default IO");
                }
            }
        }
    }
}
