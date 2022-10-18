using MeasureApp.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MeasureApp.Model.Common;
using MeasureApp.ViewModel.Common;
using System.Threading;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 串口选择
        private string tempControlPortNameSelectedValue;
        public string TempControlPortNameSelectedValue
        {
            get => tempControlPortNameSelectedValue;
            set
            {
                tempControlPortNameSelectedValue = value;
                RaisePropertyChanged(() => TempControlPortNameSelectedValue);
            }
        }

        // 开启温度监控
        private bool tempControlMonitorIsOpen;
        public bool TempControlMonitorIsOpen
        {
            get => tempControlMonitorIsOpen;
            set
            {
                tempControlMonitorIsOpen = value;
                RaisePropertyChanged(() => TempControlMonitorIsOpen);
            }
        }

        // 当前温度
        private double tempControlCurrentTempText;
        public double TempControlCurrentTempText
        {
            get => tempControlCurrentTempText;
            set
            {
                tempControlCurrentTempText = value;
                RaisePropertyChanged(() => TempControlCurrentTempText);
            }
        }

        //  设置目标温度
        private double tempControlSetTempText;
        public double TempControlSetTempText
        {
            get => tempControlSetTempText;
            set
            {
                tempControlSetTempText = value;
                RaisePropertyChanged(() => TempControlSetTempText);
            }
        }

        // 3458A 同步电压显示事件
        private CancellationTokenSource tempControlTokenSource = new();


        /// <summary>
        /// 温度控制开启与关闭
        /// </summary>
        public void TempControlMonitorStatusChanged()
        {
            try
            {
                if (!TempControlMonitorIsOpen)
                {
                    tempControlTokenSource.Cancel();
                }
                else
                {
                    tempControlTokenSource = new();

                    _ = Task.Run(() =>
                    {
                        try
                        {
                            while (!tempControlTokenSource.IsCancellationRequested)
                            {
                                SerialPortsInstance.WriteString(TempControlPortNameSelectedValue, "TEMP?\n");
                                string retTemp = SerialPortsInstance.ReadLine(TempControlPortNameSelectedValue);
                                TempControlCurrentTempText = Convert.ToDouble(retTemp);
                                Thread.Sleep(500);
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                            tempControlTokenSource.Cancel();
                            TempControlMonitorIsOpen = false;
                        }
                    }, tempControlTokenSource.Token);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }


        private void TempControlSetTemp()
        {
            try
            {
                if (TempControlSetTempText > 155 || TempControlSetTempText < -65)
                {
                    MessageBoxResult result = MessageBox.Show($"确认温度:{TempControlSetTempText}摄氏度后继续", "非常规温度确认", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.Cancel)
                        return;
                }
                SerialPortsInstance.WriteString(TempControlPortNameSelectedValue, $"SETP {TempControlSetTempText}\n");
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }


        // CommandBase

        private CommandBase tempControlMonitorStatusChangedEvent;
        public CommandBase TempControlMonitorStatusChangedEvent
        {
            get
            {
                if (tempControlMonitorStatusChangedEvent == null)
                {
                    tempControlMonitorStatusChangedEvent = new CommandBase(new Action<object>(_ => TempControlMonitorStatusChanged()));
                }
                return tempControlMonitorStatusChangedEvent;
            }
        }

        private CommandBase tempControlSetTempEvent;
        public CommandBase TempControlSetTempEvent
        {
            get
            {
                if (tempControlSetTempEvent == null)
                {
                    tempControlSetTempEvent = new CommandBase(new Action<object>(_ => TempControlSetTemp()));
                }
                return tempControlSetTempEvent;
            }
        }
    }
}
