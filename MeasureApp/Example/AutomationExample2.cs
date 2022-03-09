/// lang=C#
using System;
using System.Windows;
using System.Linq;
using MeasureApp;
using MeasureApp.ViewModel;
using MeasureApp.Model;

public class Automation
{
    public int Main(MainWindowDataContext dataContext)
    {
        try
        {
            // 引用上下文
            GPIB3458AMeasure m3458A = dataContext.Measure3458AInstance;
            SerialPorts serialPorts = dataContext.SerialPortsInstance;
            DataStorage dataStorage = dataContext.DataStorageInstance;

            // 检测设备
            if (serialPorts.SerialPortNames.Count() != 1)
                throw new NullReferenceException("串口未打开");
            if (!m3458A.IsOpen)
                throw new NullReferenceException("3458A未打开");

            // 配置
            int LoopTimes = 100;
            string SendCommandHex = "02A600";
            string StorageKey = dataContext.Key3458AString;
            string serialPortName = serialPorts.SerialPortNames.First();
            byte[] SendCommandBytes = Utility.Hex2Bytes(SendCommandHex);
            decimal voltage;

            // 丢弃现有缓存
            dataContext.StatusBarText = $"A/{LoopTimes}";
            _ = m3458A.ReadDecimal();
            _ = m3458A.ReadDecimal();

            // 向DAC下位机发送电压命令
            dataContext.StatusBarText = $"B/{LoopTimes}";
            serialPorts.WriteBytes(serialPortName, SendCommandBytes);

            // 采集过程 
            dataContext.ProgressStopWatchStart(LoopTimes);
            for (int i = 0; i < LoopTimes; i++)
            {
                dataContext.StatusBarText = $"C{i + 1}/{LoopTimes}";

                // 从3458A接收自动采集的电压命令
                voltage = m3458A.ReadDecimal();

                // 存储电压
                dataStorage.AddData(StorageKey, voltage);

                // 进度更新与任务停止检测
                dataContext.ProgressStopWatchUpdate(i);
                if (dataContext.IsAutomationCancelled)
                    return -2;
            }
            dataContext.ProgressStopWatchStop();
            return 0;
        }
        catch (Exception ex)
        {
            _ = MessageBox.Show(ex.ToString());
            return -1;
        }
    }
}
