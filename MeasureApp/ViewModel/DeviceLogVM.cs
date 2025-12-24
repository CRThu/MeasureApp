using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DryIoc;
using MeasureApp.Messages;
using MeasureApp.Services;
using Microsoft.Win32;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.ViewModel
{
    public partial class DeviceLogVM : BaseVM
    {
        private readonly CommandLogService _commandLogger;
        public CommandLogService CommandLogger => _commandLogger;

        [ObservableProperty]
        private bool isLogAutoScroll = true;

        public DeviceLogVM(CommandLogService commandLogger)
        {
            Title = "通信日志";
            ContentId = "DeviceLog";
            _commandLogger = commandLogger;
        }

        [RelayCommand]
        public void CleanLog()
        {
            try
            {
                CommandLogger.Clear();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void SaveLog()
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    Title = "保存日志",
                    FileName = $"Log.{DateTime.Now:yyyyMMddHHmmss}.log",
                    DefaultExt = ".log",
                    Filter = "Log File (*.log)|*.log|All Files (*.*)| *.*"
                };

                if (sfd.ShowDialog() == true)
                {
                    var logs = CommandLogger.Logs.ToList();
                    using (StreamWriter sw = new StreamWriter(sfd.FileName))
                    {
                        foreach (var log in logs)
                        {
                            sw.WriteLine(log.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }
    }
}
