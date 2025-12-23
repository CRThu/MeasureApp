using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DryIoc;
using MeasureApp.Messages;
using MeasureApp.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowVM : BaseVM
    {
        private readonly AppContextManager _context;

        public AppContextManager Context => _context;

        public string AppName => $"MeasureApp {Assembly.GetEntryAssembly().GetName().Version}";

        [ObservableProperty]
        private string statusBarText = "Text from MainWindowVM";

        public MainWindowVM(AppContextManager context)
        {
            _context = context;
        }

        [RelayCommand]
        public void MainWindowLoaded()
        {
            try
            {
                //MessageBox.Show("MainWindowLoaded");
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void MainWindowClosing()
        {
            try
            {
                _context.Configs.AppConfig.PresetCommands = App.Locator.DeviceDebug.Presets;
                _context.Configs.Update();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void MainWindowClosed()
        {
            try
            {
                _context?.Dispose();
                //MessageBox.Show("MainWindowClosed");
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void SaveLayout()
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "Layout Files (*.xml)|*.xml|All files (*.*)|*.*",
                    Title = "保存布局文件",
                    FileName = "layout.xml"
                };

                if (sfd.ShowDialog() == true)
                {
                    WeakReferenceMessenger.Default.Send(new SaveLayoutMessage(sfd.FileName));
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void RestoreLayout()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Filter = "Layout Files (*.xml)|*.xml|All files (*.*)|*.*",
                    Title = "加载布局文件"
                };

                if (ofd.ShowDialog() == true)
                {
                    if (!File.Exists(ofd.FileName))
                    {
                        MessageBox.Show("布局文件不存在。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    WeakReferenceMessenger.Default.Send(new RestoreLayoutMessage(ofd.FileName));
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

    }
}
