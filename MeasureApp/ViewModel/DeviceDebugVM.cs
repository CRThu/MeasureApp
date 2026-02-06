using CarrotLink.Core.Session;
using CarrotLink.Core.Utility;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DryIoc;
using MeasureApp.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MeasureApp.ViewModel
{
    public partial class PresetCommandItem : ObservableObject
    {
        [ObservableProperty]
        private string text = "";

        [ObservableProperty]
        private bool isPeriodicSendActive = false;
    }

    public partial class DesignDeviceDebugVM : BaseVM
    {
        [ObservableProperty]
        private ObservableCollection<PresetCommandItem> presets = new ObservableCollection<PresetCommandItem>()
        {
            new PresetCommandItem() { Text = "<ITEM 0>", IsPeriodicSendActive = true },
            new PresetCommandItem() { Text = "<ITEM 1>" },
            new PresetCommandItem() { Text = "<ITEM 2>" },
            new PresetCommandItem() { Text = "<ITEM 3>" },
            new PresetCommandItem() { Text = "<ITEM 4>" },
            new PresetCommandItem() { Text = "<ITEM 5>" }
        };
    }

    public partial class DeviceDebugVM : BaseVM
    {
        private readonly DeviceManager _deviceManager;
        private readonly ConfigManager _configManager;
        public DeviceManager DeviceManager => _deviceManager;
        public ConfigManager ConfigManager => _configManager;

        [ObservableProperty]
        private ConnectionInfo selectedDevice;

        [ObservableProperty]
        private ObservableCollection<PresetCommandItem> presets;

        [ObservableProperty]
        private string debugCommandText = "ping();";

        [ObservableProperty]
        private bool isPeriodicSendEnabled = false;

        [ObservableProperty]
        private int periodIntervalMs = 1000;

        private CancellationTokenSource _periodicCts;

        public DeviceDebugVM(DeviceManager deviceManager, ConfigManager configManager)
        {
            Title = "设备调试";
            ContentId = "DeviceDebug";
            _deviceManager = deviceManager;
            _configManager = configManager;

            Presets = new ObservableCollection<PresetCommandItem>();
            // 读取默认appconfig保存的预设
            if (ConfigManager.AppConfig.PresetCommands != null)
            {
                foreach (var item in ConfigManager.AppConfig.PresetCommands)
                {
                    Presets.Add(item);
                }
            }
        }

        // 当 IsPeriodicSendEnabled 改变时触发
        partial void OnIsPeriodicSendEnabledChanged(bool value)
        {
            if (value)
            {
                _periodicCts = new CancellationTokenSource();
                _ = RunPeriodicSendTask(_periodicCts.Token);
            }
            else
            {
                _periodicCts?.Cancel();
                _periodicCts?.Dispose();
                _periodicCts = null;
            }
        }

        // 周期性发送任务逻辑
        private async Task RunPeriodicSendTask(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    string deviceName = null;
                    List<string> activeCommands = null;

                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        if (SelectedDevice != null)
                        {
                            deviceName = SelectedDevice.Name;
                            activeCommands = Presets.Where(p => p.IsPeriodicSendActive && !string.IsNullOrWhiteSpace(p.Text))
                            .Select(p => p.Text)
                            .ToList();
                        }
                    }, DispatcherPriority.Background);

                    if (activeCommands != null && deviceName != null && activeCommands.Count > 0)
                    {
                        foreach (var cmd in activeCommands)
                        {
                            if (token.IsCancellationRequested)
                                break;

                            // 执行发送逻辑
                            await DeviceManager[deviceName].SendAscii(cmd + "\n").ConfigureAwait(false);

                            // 每个命令发送后的延迟
                            await Task.Delay(PeriodIntervalMs, token).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        await Task.Delay(PeriodIntervalMs, token).ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // 正常停止
            }
            catch (Exception ex)
            {
                // 异常停止，更新UI状态
                IsPeriodicSendEnabled = false;
                // MessageBox 可能会在非UI线程弹出，此处简单处理
                Application.Current.Dispatcher.Invoke(() => MessageBox.Show($"周期发送异常: {ex.Message}"));
            }
        }

        [RelayCommand]
        public void SendPreset(object parameter)
        {
            try
            {
                string cmd = (parameter as string) ?? ((parameter as PresetCommandItem)?.Text);

                if (cmd != null)
                {
                    if (SelectedDevice != null)
                    {
                        DeviceManager[SelectedDevice.Name].SendAscii(cmd + "\n");
                    }
                    else
                    {
                        MessageBox.Show("未选择设备");
                    }
                }
                else
                {
                    MessageBox.Show("无法发送预设命令");
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void AddPreset()
        {
            try
            {
                Presets.Add(new PresetCommandItem() { Text = "" });
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void RemovePreset(object parameter)
        {
            try
            {
                if (parameter is PresetCommandItem item)
                {
                    Presets.Remove(item);
                }
                else
                {
                    MessageBox.Show("无法删除预设命令");
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void LoadPresets()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog()
                {
                    FileName = "cmd_presets.json",
                    DefaultExt = ".json",
                };
                if (ofd.ShowDialog() == true)
                {
                    PresetCommandItem[] p = SerializationHelper.DeserializeFromFile<PresetCommandItem[]>(ofd.FileName);
                    if (p != null)
                    {
                        Presets.Clear();
                        foreach (var item in p)
                        {
                            Presets.Add(item);
                        }
                    }
                    else
                    {
                        MessageBox.Show("无法加载预设命令文件");
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void SavePresets()
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    FileName = "cmd_presets.json",
                    DefaultExt = ".json",
                };
                if (sfd.ShowDialog() == true)
                {
                    SerializationHelper.SerializeToFile(Presets, sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }
    }
}
