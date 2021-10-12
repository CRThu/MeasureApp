using MeasureApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 搜索到的GPIB设备地址
        private ObservableCollection<string> gpibDevicesName = new ObservableCollection<string>();
        public ObservableCollection<string> GpibDevicesName
        {
            get => gpibDevicesName;
            set
            {
                gpibDevicesName = value;
                RaisePropertyChanged(() => GpibDevicesName);
            }
        }

        // 打开GPIB的设备ID读取
        private string gpibDeviceConnectStatusText = $"No Device Connected.";
        public string GpibDeviceConnectStatusText
        {
            get => gpibDeviceConnectStatusText;
            set
            {
                gpibDeviceConnectStatusText = value;
                RaisePropertyChanged(() => GpibDeviceConnectStatusText);
            }
        }


        // 搜索GPIB设备事件
        private CommandBase gpibDeviceSearchEvent;
        public CommandBase GpibDeviceSearchEvent
        {
            get
            {
                if (gpibDeviceSearchEvent == null)
                {
                    gpibDeviceSearchEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            GpibDevicesName.Clear();
                            GPIB.SearchDevices("GPIB?*INSTR").ToList().ForEach(dev => GpibDevicesName.Add(dev));

                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return gpibDeviceSearchEvent;
            }
        }

        // 打开GPIB设备事件
        private CommandBase gpibDeviceOpenEvent;
        public CommandBase GpibDeviceOpenEvent
        {
            get
            {
                if (gpibDeviceOpenEvent == null)
                {
                    gpibDeviceOpenEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            if (param is string)
                            {
                                gpibDeviceConnectStatusText = $"No Device Connected.";
                                Measure3458AInstance.Dispose();
                                gpibDeviceConnectStatusText = $"{Measure3458AInstance.Open(param as string)} Connected.";
                                Measure3458AInstance.Timeout = Properties.Settings.Default.GPIBTimeout;
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return gpibDeviceOpenEvent;
            }
        }
    }
}
