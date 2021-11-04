using MeasureApp.Model;
using OxyPlot.Series;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        string Key3458AString = "3458A Data Storage";
        string KeySerialPortString = "Serial Port Data Storage";

        public MainWindowDataContext()
        {
            // 添加默认Key
            dataStorageInstance.AddKey(Key3458AString);
            dataStorageInstance.AddKey(KeySerialPortString);
            dataStorageSelectedValue = Key3458AString;

            // TEST
            PlotViewPlotModel.Series.Add(new LineSeries());
            //var lineSeries = plotViewPlotModel.Series[0] as LineSeries;
            //lineSeries.Points.Add(new OxyPlot.DataPoint(1, 1));
            //lineSeries.Points.Add(new OxyPlot.DataPoint(2, 4));
            //lineSeries.Points.Add(new OxyPlot.DataPoint(3, 9));
            //lineSeries.Points.Add(new OxyPlot.DataPoint(4, 16));
            //lineSeries.Points.Add(new OxyPlot.DataPoint(5, 25));
        }

        // 3458A 通信类
        private GPIB3458AMeasure measure3458AInstance = new();
        public GPIB3458AMeasure Measure3458AInstance
        {
            get => measure3458AInstance;
            set
            {
                measure3458AInstance = value;
                RaisePropertyChanged(() => Measure3458AInstance);
            }
        }

        // 多串口通信类
        private SerialPorts serialPortsInstance = new();
        public SerialPorts SerialPortsInstance
        {
            get => serialPortsInstance;
            set
            {
                serialPortsInstance = value;
                RaisePropertyChanged(() => SerialPortsInstance);
            }
        }

        // 数据存储类
        private DataStorage dataStorageInstance = new();
        public DataStorage DataStorageInstance
        {
            get => dataStorageInstance;
            set
            {
                dataStorageInstance = value;
                RaisePropertyChanged(() => DataStorageInstance);
            }
        }

        // MainWindow关闭事件
        private CommandBase mainWindowLoadedEvent;
        public CommandBase MainWindowLoadedEvent
        {
            get
            {
                if (mainWindowLoadedEvent == null)
                {
                    mainWindowLoadedEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            GpibDeviceSearchEvent.Execute(null);
                            SerialPortDeviceSearchEvent.Execute(null);
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return mainWindowLoadedEvent;
            }
        }

        // MainWindow关闭事件
        private CommandBase mainWindowClosedEvent;
        public CommandBase MainWindowClosedEvent
        {
            get
            {
                if (mainWindowClosedEvent == null)
                {
                    mainWindowClosedEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            Measure3458AInstance.Dispose();
                            SerialPortsInstance.CloseAll();
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return mainWindowClosedEvent;
            }
        }

        // 状态栏
        private string statusBarText = "StatusBar";
        public string StatusBarText
        {
            get => statusBarText;
            set
            {
                statusBarText = value;
                RaisePropertyChanged(() => StatusBarText);
            }
        }

        // 通用DataGrid自动添加行号
        private CommandBase dataGridLoadingRowAddRowIndexEvent;
        public CommandBase DataGridLoadingRowAddRowIndexEvent
        {
            get
            {
                if (dataGridLoadingRowAddRowIndexEvent == null)
                {
                    dataGridLoadingRowAddRowIndexEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            if (param is DataGridRowEventArgs)
                            {
                                var row = (param as DataGridRowEventArgs).Row;
                                row.Header = (row.GetIndex() + 1).ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return dataGridLoadingRowAddRowIndexEvent;
            }
        }
    }
}
