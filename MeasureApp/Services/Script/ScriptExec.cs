using CarrotLink.Core.Session;
using CommunityToolkit.Mvvm.ComponentModel;
using MeasureApp.Model.Script;
using MeasureApp.Model.SerialPortScript;
using MeasureApp.Services.ScriptLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeasureApp.Services.Script
{
    public partial class ScriptExec : ObservableObject
    {
        public readonly string EnvDefaultIOName = "Env::Default::IO";
        public readonly string EnvDefaultMeasureName = "Env::Default::Measure";

        // A dictionary to hold registered script commands
        private readonly Dictionary<string, IScriptCommand> _commands = new();

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
        private readonly ScriptEnvironment _environment;
        private readonly ScriptContext _scriptContext;

        public ScriptEnvironment Environment => _environment;

        public ScriptExec(AppContextManager context)
        {
            _context = context;
            _environment = new ScriptEnvironment();
            _scriptContext = new ScriptContext(_context, _environment);

            // Automatically register commands on initialization
            RegisterCommands();
        }

        /// <summary>
        /// Uses reflection to find and register all classes that implement IScriptCommand.
        /// </summary>
        private void RegisterCommands()
        {
            var commandTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IScriptCommand).IsAssignableFrom(t));

            foreach (var type in commandTypes)
            {
                var attr = type.GetCustomAttribute<ScriptCommandAttribute>();
                if (attr != null)
                {
                    var commandInstance = (IScriptCommand)Activator.CreateInstance(type);
                    _commands.Add(attr.CommandName, commandInstance);
                    Console.WriteLine($"Registered script command: '{attr.CommandName}' -> {type.Name}");
                }
            }
        }

        public void Step()
        {
            if (!IsRunning)
            {
                RunOneLineAsync().GetAwaiter().GetResult();

                if (CurrentLine > ScriptLines?.Length)
                    CurrentLine = 1;
            }
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            // todo
            _exec = Task.Run(ExecTaskAsync);
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

        private async Task RunOneLineAsync()
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
                    await EmitAsync(scriptLine);
            }

            CurrentLine++;

            // 跳过后空行
            SkipEmptyLine();
        }

        public async Task ExecTaskAsync()
        {
            try
            {
                IsRunning = true;
                while (CurrentLine <= ScriptLines?.Length)
                {
                    if (_cts.IsCancellationRequested)
                        return;

                    await RunOneLineAsync();

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

        /// <summary>
        /// The refactored Emit method. It now dispatches commands instead of handling them directly.
        /// </summary>
        private async Task EmitAsync(string code)
        {
            if (XmlTag.IsMatchXmlTag(code))
            {
                var attributes = XmlTag.GetXmlTagAttrs(code);
                var parameters = new CommandParameters(attributes);
                string commandName = parameters.CommandName.ToUpperInvariant();

                if (_commands.TryGetValue(commandName, out IScriptCommand command))
                {
                    // Found a registered command, execute it
                    await command.ExecuteAsync(_scriptContext, parameters);
                }
                else
                {
                    // Command not found, you can log a warning or throw an exception
                    Console.WriteLine($"Warning: Unknown script command '{commandName}'");
                    // Optionally, you could fall back to the default IO behavior here too
                }
            }
            else
            {
                // Default operation for non-tagged commands
                if (_environment.TryGet<string>(EnvDefaultIOName, out var defaultIOKey) && !string.IsNullOrEmpty(defaultIOKey))
                {
                    await _context.Devices[defaultIOKey].SendAscii(code + "\n");
                }
                else
                {
                    throw new InvalidOperationException("No default IO device specified for raw command.");
                }
            }
        }
    }
}
