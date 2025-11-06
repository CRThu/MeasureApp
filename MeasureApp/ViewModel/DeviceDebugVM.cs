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

namespace MeasureApp.ViewModel
{
    public partial class PresetCommandItem : ObservableObject
    {
        [ObservableProperty]
        private string text = "";
    }

    public partial class DesignDeviceDebugVM : BaseVM
    {
        [ObservableProperty]
        private ObservableCollection<PresetCommandItem> presets = new ObservableCollection<PresetCommandItem>()
        {
            new PresetCommandItem() { Text = "<ITEM 0>"},
            new PresetCommandItem() { Text = "<ITEM 1>"},
            new PresetCommandItem() { Text = "<ITEM 2>"},
            new PresetCommandItem() { Text = "<ITEM 3>"},
            new PresetCommandItem() { Text = "<ITEM 4>"},
            new PresetCommandItem() { Text = "<ITEM 5>"}
        };
    }

    public partial class DeviceDebugVM : BaseVM
    {
        private readonly AppContextManager _context;
        public AppContextManager Context => _context;

        [ObservableProperty]
        private ConnectionInfo selectedDevice;

        [ObservableProperty]
        private ObservableCollection<PresetCommandItem> presets;

        [ObservableProperty]
        private string debugCommandText = "OPEN;";

        public DeviceDebugVM(AppContextManager context)
        {
            _context = context;

            Presets = new ObservableCollection<PresetCommandItem>();
            // 读取默认appconfig保存的预设
            if (Context.Configs.AppConfig.PresetCommands != null)
            {
                foreach (var item in Context.Configs.AppConfig.PresetCommands)
                {
                    Presets.Add(item);
                }
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
                        Context.Devices[SelectedDevice.Name].SendAscii(cmd + "\n");
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
