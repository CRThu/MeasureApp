using CarrotLink.Core.Session;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DryIoc;
using MeasureApp.Model.Common;
using MeasureApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.ViewModel
{
    public partial class DeviceDebugVM : BaseVM
    {
        private readonly AppContextManager _context;
        public AppContextManager Context => _context;

        [ObservableProperty]
        private ConnectionInfo selectedDevice;

        [ObservableProperty]
        private bool isLogAutoScroll = true;

        [ObservableProperty]
        private string commandPacketText = "OPEN;";

        public DeviceDebugVM(AppContextManager context)
        {
            _context = context;
        }

        [RelayCommand]
        public void SendCommandPacket()
        {
            try
            {
                if (SelectedDevice != null)
                {
                    Context.Devices[SelectedDevice.Name].SendAscii(CommandPacketText + "\n");
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void CleanLog()
        {
            try
            {
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
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }
    }
}
