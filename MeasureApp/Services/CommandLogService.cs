using CarrotLink.Core.Logging;
using CarrotLink.Core.Protocols.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using MeasureApp.Model.Log;
using MeasureApp.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace MeasureApp.Services
{

    public partial class CommandLogService : ObservableObject, IPacketLogger
    {
        private readonly BlockingCollection<CommandLogEntry> _logBuffer = new BlockingCollection<CommandLogEntry>();

        private readonly Task _consumerTask;
        private readonly CancellationTokenSource _cts;
        private readonly object _logsLock = new object();

        [ObservableProperty]
        private ObservableRangeCollection<CommandLogEntry> logs;

        public CommandLogService()
        {
            logs = new ObservableRangeCollection<CommandLogEntry>();
            BindingOperations.EnableCollectionSynchronization(Logs, _logsLock);

            _cts = new CancellationTokenSource();
            _consumerTask = Task.Run(() => ProcessLog(_cts.Token));
        }

        private void ProcessLog(CancellationToken token)
        {
            try
            {
                var batch = new List<CommandLogEntry>();

                // GetConsumingEnumerable 会高效地阻塞线程，直到有新项或任务被取消
                foreach (var item in _logBuffer.GetConsumingEnumerable(token))
                {
                    batch.Add(item);

                    // 贪婪地获取当前队列中的所有其他项，以形成一个更大的批次
                    while (_logBuffer.TryTake(out var nextItem))
                    {
                        batch.Add(nextItem);
                    }

                    // 将收集到的整个批次调度到UI线程进行一次性更新
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        lock (_logsLock)
                        {
                            Logs.AddRange(batch);
                        }
                    });

                    // 清空批处理列表以备下次使用
                    batch.Clear();
                }
            }
            catch (OperationCanceledException)
            {
                // This is expected when Dispose is called.
            }
            catch (Exception ex)
            {
                // Log any unexpected errors from the background task.
                Debug.WriteLine($"Error in CommandLogService consumer task: {ex}");
            }
        }

        public void HandlePacket(IPacket packet, string sender)
        {
            if (_cts.IsCancellationRequested)
                return;

            CommandLogEntry logEntry = new CommandLogEntry()
            {
                TimeStamp = DateTime.Now,
                Sender = sender ?? "<sender>",
                Message = packet.ToString()
            };

            _logBuffer.Add(logEntry);
        }

        public void Dispose()
        {
            _cts.Cancel();
            _logBuffer.CompleteAdding(); // 告知集合不会再有新项，以解除任何阻塞

            try
            {
                // 等待任务完成，可以设置一个短暂的超时
                _consumerTask.Wait(TimeSpan.FromMilliseconds(500));
            }
            catch (AggregateException ex)
            {
                // 忽略因取消而引发的异常
                ex.Handle(e => e is OperationCanceledException);
            }
            finally
            {
                _cts.Dispose();
            }
        }
    }
}
