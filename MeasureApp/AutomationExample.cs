﻿/// lang=C#
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
        string Key3458AString = "3458A Data Storage";

        GPIB3458AMeasure m3458A = dataContext.Measure3458AInstance;
        SerialPorts serialPorts = dataContext.SerialPortsInstance;
        DataStorage dataStorage = dataContext.DataStorageInstance;

        string serialPortName = serialPorts.SerialPortNames.First();
        int delayClock = 2;
        int LoopTimes = 262144;
        byte[] SendCommandByteText = Utility.ToBytesFromHexString("02A600");
        decimal voltage;

        if (!m3458A.IsOpen)
        {
            throw new NullReferenceException("3458A未打开");
        }

        // 丢弃现有缓存
        _ = m3458A.ReadDecimal();
        _ = m3458A.ReadDecimal();

        // 采集过程
        for (int i = 0; i < LoopTimes; i++)
        {
            // 向DAC下位机发送电压命令
            dataContext.StatusBarText = $"{i + 1}A/{LoopTimes}";
            serialPorts.WriteBytes(serialPortName, SendCommandByteText, SendCommandByteText.Length);

            // 等待delay拍数，期间采集的数据丢弃
            dataContext.StatusBarText = $"{i + 1}B/{LoopTimes}";
            for (int j = 0; j < delayClock; j++)
            {
                _ = m3458A.ReadDecimal();
            }

            // 从3458A接收自动采集的电压命令
            dataContext.StatusBarText = $"{i + 1}C/{LoopTimes}";
            voltage = m3458A.ReadDecimal();

            // 存储电压
            dataContext.StatusBarText = $"{i + 1}D/{LoopTimes}";
            dataStorage.AddData(Key3458AString, voltage);
        }
        return 0;
    }
}