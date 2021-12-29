/// lang=C#
using System;
using System.Windows;
using System.Linq;
using MeasureApp;
using MeasureApp.ViewModel;
using MeasureApp.Model;
using System.Threading;

public class Automation
{
    // AD7124 IOUT电流高温测试
    // 3458A Local调至1mA挡
    // 上位机设置采样速率1NPLC
    // 连接万用表至IN12或IN13脚
    public int Main(MainWindowDataContext dataContext)
    {
        try
        {
            // 程序上下文
            GPIB3458AMeasure m3458A = dataContext.Measure3458AInstance;
            SerialPorts serialPorts = dataContext.SerialPortsInstance;
            DataStorage dataStorage = dataContext.DataStorageInstance;

            // 检测设备
            if (serialPorts.SerialPortNames.Count() != 1)
                throw new NullReferenceException("串口未打开");
            if (!m3458A.IsOpen)
                throw new NullReferenceException("3458A未打开");

            // 获取设备串口
            string serialPortName = serialPorts.SerialPortNames.First();

            SetInit(serialPorts, serialPortName);

            for (int i = 0;; i++)
            {
                string time = GetTime();
                double temp = GetTemp(serialPorts, serialPortName);
                decimal current;

                // 记录电流
                dataStorage.AddData("time", time);
                dataStorage.AddData("temp", temp);

                SetIout(serialPorts, serialPortName, 1);
                current = GetCurrent(m3458A);
                dataStorage.AddData("50uA", current);

                SetIout(serialPorts, serialPortName, 2);
                current = GetCurrent(m3458A);
                dataStorage.AddData("100uA", current);

                SetIout(serialPorts, serialPortName, 3);
                current = GetCurrent(m3458A);
                dataStorage.AddData("250uA", current);

                SetIout(serialPorts, serialPortName, 4);
                current = GetCurrent(m3458A);
                dataStorage.AddData("500uA", current);

                SetIout(serialPorts, serialPortName, 5);
                current = GetCurrent(m3458A);
                dataStorage.AddData("750uA", current);

                SetIout(serialPorts, serialPortName, 6);
                current = GetCurrent(m3458A);
                dataStorage.AddData("1000uA", current);

                dataContext.StatusBarText = $"{i} | {time} : {temp} deg";

                //Thread.Sleep(1000);
                if (dataContext.IsAutomationCancelled)
                    return -2;
            }

            return 0;
        }
        catch (Exception ex)
        {
            _ = MessageBox.Show(ex.ToString());
            return -1;
        }
    }

    public static void SetInit(SerialPorts serialPorts, string serialPortName)
    {
        // 初始化
        serialPorts.WriteString(serialPortName, $"RESET;");
        Thread.Sleep(100);
        // CH0温度传感器配置
        serialPorts.WriteString(serialPortName, $"READ.TEMP.INIT;");
        Thread.Sleep(100);
        // IOUT0,IOUT1配置CH12 CH13
        serialPorts.WriteString(serialPortName, $"REGM;3;0;4;C;");
        Thread.Sleep(100);
        serialPorts.WriteString(serialPortName, $"REGM;3;4;4;D;");
        Thread.Sleep(100);
    }

    public static string GetTime()
    {
        return DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff");
    }

    public static double GetTemp(SerialPorts serialPorts, string serialPortName)
    {
        // 获取设备温度
        serialPorts.ReadExistingString(serialPortName);
        serialPorts.WriteString(serialPortName, "READ.TEMP.DATA;");
        while (serialPorts.SerialPortsDict[serialPortName].BytesToRead == 0) ;
        string retData = serialPorts.ReadExistingString(serialPortName);
        retData = retData.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1].Trim();
        return Convert.ToDouble(retData);
    }

    public static void SetIout(SerialPorts serialPorts, string serialPortName, int iout)
    {
        serialPorts.WriteString(serialPortName, $"REGM;3;8;3;{iout};");
        Thread.Sleep(20);
        serialPorts.WriteString(serialPortName, $"REGM;3;11;3;{iout};");
        Thread.Sleep(20);
    }

    public static decimal GetCurrent(GPIB3458AMeasure m3458A)
    {
        _ = m3458A.ReadDecimal();
        return m3458A.ReadDecimal();
    }
}
