using CarrotLink.Core.Session;
using CommunityToolkit.Mvvm.ComponentModel;
using MeasureApp.Model.Script;
using NCalc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeasureApp.Services.Script
{
    public partial class ScriptExecutor : ObservableObject
    {
        public static readonly string EnvDefaultIOName = "Env::Default::IO";
        public static readonly string EnvDefaultMeasureName = "Env::Default::Measure";

        // 正则表达式用于匹配 {expression:format} 或 {expression}
        private static readonly Regex _inlineExpressionRegex = new Regex(@"\{([^}]+)\}", RegexOptions.Compiled);

        // A dictionary to hold registered script commands
        private readonly Dictionary<string, IScriptCommand> _commands = new();
        private readonly Stack<ControlFlowState> _controlFlow = new();

        [ObservableProperty]
        private int interval = 500;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ScriptLines))]
        private string scriptCode;

        public string[] ScriptLines => ScriptCode?.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

        [ObservableProperty]
        private int currentLine = 1;

        private volatile ScriptExecutionMode _executionMode = ScriptExecutionMode.None;
        private int _isRunningFlag = 0;

        public bool IsRunning => _isRunningFlag == 1;

        private Task _exec;
        private CancellationTokenSource _cts;

        private readonly AppContextManager _context;
        private readonly ScriptEnvironment _environment;
        private readonly ScriptContext _scriptContext;

        public ScriptEnvironment Environment => _environment;

        public ScriptExecutor(AppContextManager context)
        {
            _context = context;
            _environment = new ScriptEnvironment();
            _scriptContext = new ScriptContext(_context, _environment, this);

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

        private void SetIsRunning(bool value)
        {
            int newVal = value ? 1 : 0;
            int oldVal = Interlocked.Exchange(ref _isRunningFlag, newVal);
            if (oldVal != newVal)
            {
                OnPropertyChanged(nameof(IsRunning));
            }
        }

        public void Start(ScriptExecutionMode mode)
        {
            if (IsRunning)
                return;

            if (ScriptLines == null || ScriptLines.Length == 0)
                return;

            if (CurrentLine > ScriptLines.Length)
            {
                MessageBox.Show("current line out of range.");
                return;
            }

            _executionMode = mode;
            EnsureTaskIsRunning();
        }

        private void EnsureTaskIsRunning()
        {
            if (!IsRunning)
            {
                SetIsRunning(true);
                _cts = new CancellationTokenSource();
                _exec = Task.Run(ExecTaskAsync, _cts.Token);
            }
        }

        public void Stop()
        {
            _cts?.Cancel();
        }

        public void Reset()
        {
            if (IsRunning)
                return;

            CurrentLine = 1;
            //_environment.Clear();
            _controlFlow.Clear();
        }

        private void SkipEmptyLine()
        {
            while (CurrentLine <= ScriptLines?.Length
                && string.IsNullOrWhiteSpace(ScriptLines[CurrentLine - 1]))
                CurrentLine++;
        }

        public async Task ExecTaskAsync()
        {
            try
            {
                while (IsRunning)
                {
                    _cts.Token.ThrowIfCancellationRequested();

                    if (CurrentLine > ScriptLines?.Length)
                    {
                        break;
                    }

                    await RunOneLineAsync(_cts.Token);

                    if (_executionMode == ScriptExecutionMode.Step)
                    {
                        break;
                    }

                    if (IsRunning)
                    {
                        await Task.Delay(Interval, _cts.Token);
                    }
                }
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error on line {CurrentLine}: {ex}");
            }
            finally
            {
                SetIsRunning(false);
                if (CurrentLine > ScriptLines?.Length)
                {
                    CurrentLine = 1;
                }
            }
        }

        private async Task RunOneLineAsync(CancellationToken cancellationToken)
        {
            // 跳过前空行
            SkipEmptyLine();

            // 运行当前行
            if (CurrentLine > ScriptLines?.Length)
                return;

            // 若存在注释则滤除注释
            string scriptLine = ScriptLines[CurrentLine - 1].Split('#', 2).First().Trim();

            ExecutionDirective directive = string.IsNullOrEmpty(scriptLine)
                ? ContinueExecution.Instance
                : await EmitAsync(scriptLine, cancellationToken);

            // --- Process the returned directive ---
            switch (directive)
            {
                case ContinueExecution:
                    CurrentLine++;
                    break;
                case JumpToLine jump:
                    CurrentLine = jump.TargetLine;
                    break;
                case StopExecution:
                    SetIsRunning(false);
                    CurrentLine++;
                    break;
            }

            // 跳过后空行
            SkipEmptyLine();

            if (CurrentLine > ScriptLines?.Length)
            {
                SetIsRunning(false);
                CurrentLine = 1;
            }
        }


        /// <summary>
        /// The refactored Emit method. It now dispatches commands instead of handling them directly.
        /// </summary>
        private async Task<ExecutionDirective> EmitAsync(string code, CancellationToken cancellationToken)
        {
            // --- 在所有处理之前，先解析内联表达式 ---
            string evaluatedCode = EvaluateInlineExpressions(code);

            if (XmlTag.IsMatchXmlTag(evaluatedCode))
            {
                var attributes = XmlTag.GetXmlTagAttrs(evaluatedCode);
                var parameters = new CommandParameters(attributes);
                string commandName = parameters.CommandName.ToUpperInvariant();

                // --- 1. Handle internal control flow commands first ---
                switch (commandName)
                {
                    case "STOP":
                        return StopExecution.Instance;
                    case "FOR":
                        return HandleFor(parameters);
                    case "ENDFOR":
                        return HandleEndFor();
                    case "IF":
                        return HandleIf(parameters);
                    case "ELSE":
                        return HandleElse();
                    case "ENDIF":
                        return HandleEndIf();
                }

                // --- 2. If not a control flow command, dispatch to registered action commands ---
                if (_commands.TryGetValue(commandName, out IScriptCommand command))
                {
                    // Found a registered command, execute it
                    return await command.ExecuteAsync(_scriptContext, parameters, cancellationToken);
                }
                else
                {
                    MessageBox.Show($"Warning: Unknown script command '{commandName}'");
                    return ContinueExecution.Instance;
                }
            }
            else
            {
                // Default operation for non-tagged commands
                if (_environment.TryGet<string>(EnvDefaultIOName, out var defaultIOKey)
                    && !string.IsNullOrEmpty(defaultIOKey))
                {
                    await _context.Devices[defaultIOKey].SendAscii(evaluatedCode + "\n");
                }
                else
                {
                    throw new InvalidOperationException("No default IO device specified for raw command.");
                }
                return ContinueExecution.Instance;
            }
        }

        /// <summary>
        /// Creates a new NCalc Expression instance with the current script environment's variables.
        /// </summary>
        /// <param name="expressionText">The string expression to evaluate.</param>
        /// <returns>A configured NCalc Expression object.</returns>
        private Expression CreateExpression(string expressionText)
        {
            var expression = new Expression(expressionText);

            // 将脚本环境中的所有变量注入到表达式上下文中
            foreach (var variable in _environment.GetAllVariables())
            {
                expression.Parameters[variable.Key] = variable.Value;
            }

            // (可选) 添加自定义函数
            expression.EvaluateFunction += (name, args) =>
            {
                if (name.Equals("round", StringComparison.OrdinalIgnoreCase))
                {
                    args.Result = Math.Round(Convert.ToDecimal(args.Parameters[0].Evaluate()));
                }
            };

            return expression;
        }

        /// <summary>
        /// Parses a string containing inline expressions like "{i+j:X}" and evaluates them.
        /// </summary>
        /// <param name="input">The raw string from the script.</param>
        /// <returns>The processed string with expressions replaced by their results.</returns>
        private string EvaluateInlineExpressions(string input)
        {
            return _inlineExpressionRegex.Replace(input, match =>
            {
                string fullMatch = match.Groups[1].Value; // "i+j:X" or "i"
                string expressionText = fullMatch;
                string format = null;

                // 检查是否存在格式化字符串
                int colonIndex = fullMatch.LastIndexOf(':');
                if (colonIndex > 0)
                {
                    expressionText = fullMatch.Substring(0, colonIndex).Trim();
                    format = fullMatch.Substring(colonIndex + 1).Trim();
                }

                // 求值
                var expression = CreateExpression(expressionText);
                object result;
                try
                {
                    result = expression.Evaluate();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to evaluate inline expression '{{{expressionText}}}'. Error: {ex.Message}");
                }

                // 格式化输出
                if (string.IsNullOrEmpty(format))
                {
                    return result.ToString();
                }
                else
                {
                    // 支持标准的 .NET 格式化字符串 (X, D, F2, etc.)
                    if (result is IFormattable formattable)
                    {
                        // 对于十六进制，需要先转为整数
                        if (format.StartsWith("X", StringComparison.OrdinalIgnoreCase)
                        || format.StartsWith("D", StringComparison.OrdinalIgnoreCase))
                        {
                            return Convert.ToInt64(result).ToString(format);
                        }
                        else if (format.StartsWith("F", StringComparison.OrdinalIgnoreCase))
                        {
                            return Convert.ToDecimal(result).ToString(format);
                        }
                        //return formattable.ToString(format, CultureInfo.InvariantCulture);
                        throw new FormatException($"Format Error, result:{result}, format:{format}.");
                    }
                    return result.ToString();
                }
            });
        }

        #region Internal Control Flow Handlers

        /*
            <for var="i" begin="1" end="2" step="1"/>
                <for var="j" begin="1" end="3" step="1"/>
                    REGW;{i:X};{j:D};
                <forend/>
            <forend/>
        
            <if condition="true"/>
                IF.TRUE;
            <else/>
                IF.FALSE;
            <endif/>
         */
        private ExecutionDirective HandleFor(CommandParameters parameters)
        {
            var variable = parameters.Get<string>("var");
            var start = parameters.Get<double>("begin");
            var end = parameters.Get<double>("end");
            var step = parameters.Get<double>("step", 1.0);

            _environment.Set(variable, start.ToString());
            bool conditionMet = (step > 0) ? (start <= end) : (start >= end);

            if (conditionMet)
            {
                _controlFlow.Push(new ForLoopState(CurrentLine, variable, end, step));
                return ContinueExecution.Instance;
            }
            else
            {
                int endForLine = FindMatchingEndTag(CurrentLine + 1, "FOR", "ENDFOR");
                return new JumpToLine(endForLine + 1);
            }
        }

        private ExecutionDirective HandleEndFor()
        {
            if (!_controlFlow.TryPop(out var state) || state is not ForLoopState loopState)
                throw new InvalidOperationException($"ENDFOR on line {CurrentLine} has no matching FOR.");

            double currentValue = _environment.Get<double>(loopState.Variable);
            double nextValue = currentValue + loopState.Step;
            _environment.Set(loopState.Variable, nextValue.ToString());

            bool conditionMet = (loopState.Step > 0) ? (nextValue <= loopState.End) : (nextValue >= loopState.End);

            if (conditionMet)
            {
                _controlFlow.Push(loopState);
                return new JumpToLine(loopState.StartLine + 1);
            }
            else
            {
                return ContinueExecution.Instance;
            }
        }

        private ExecutionDirective HandleIf(CommandParameters parameters)
        {
            // Example: <if condition="true"> or <if condition="{var} > 5">
            string conditionExpression = parameters.Get<string>("condition");
            if (string.IsNullOrEmpty(conditionExpression))
                throw new ArgumentException("IF command requires a 'condition' parameter.");

            // 使用 NCalc 求值
            var expression = CreateExpression(conditionExpression);
            bool conditionMet = false;
            try
            {
                conditionMet = Convert.ToBoolean(expression.Evaluate());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to evaluate IF condition '{conditionExpression}'. Error: {ex.Message}");
            }

            // Check if an ELSE tag exists for this IF block
            bool elseExists = false;
            try
            {
                FindMatchingEndTag(CurrentLine + 1, "IF", "ELSE");
                elseExists = true;
            }
            catch { }

            _controlFlow.Push(new IfState(CurrentLine, conditionMet, elseExists));

            if (conditionMet)
            {
                return ContinueExecution.Instance;
            }
            else
            {
                if (elseExists)
                {
                    int elseLine = FindMatchingEndTag(CurrentLine + 1, "IF", "ELSE");
                    return new JumpToLine(elseLine + 1);
                }
                int endIfLine = FindMatchingEndTag(CurrentLine + 1, "IF", "ENDIF");
                return new JumpToLine(endIfLine + 1);
            }
        }

        private ExecutionDirective HandleElse()
        {
            if (!_controlFlow.TryPeek(out var state) || state is not IfState ifState)
                throw new InvalidOperationException($"ELSE on line {CurrentLine} has no matching IF.");

            if (ifState.ConditionMet)
            {
                int endIfLine = FindMatchingEndTag(CurrentLine + 1, "ELSE", "ENDIF");
                return new JumpToLine(endIfLine + 1);
            }
            else
            {
                return ContinueExecution.Instance;
            }
        }

        private ExecutionDirective HandleEndIf()
        {
            if (!_controlFlow.TryPop(out var state) || state is not IfState)
                throw new InvalidOperationException($"ENDIF on line {CurrentLine} has no matching IF.");

            return ContinueExecution.Instance;
        }

        private int FindMatchingEndTag(int startLine, string openTag, string closeTag)
        {
            int nestingLevel = 1;
            for (int i = startLine - 1; i < ScriptLines.Length; i++)
            {
                string line = ScriptLines[i].Split('#', 2).First().Trim();
                if (XmlTag.IsMatchXmlTag(line))
                {
                    var tag = XmlTag.GetXmlTagAttrs(line)["Tag"];
                    if (tag.Equals(openTag, StringComparison.OrdinalIgnoreCase))
                        nestingLevel++;
                    else if (tag.Equals(closeTag, StringComparison.OrdinalIgnoreCase))
                    {
                        nestingLevel--;
                        if (nestingLevel == 0)
                            return i + 1; // 1-based
                    }
                }
            }
            throw new InvalidOperationException($"Could not find matching '{closeTag}' for '{openTag}' on line {startLine - 1}.");
        }

        #endregion
    }
}
