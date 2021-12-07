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
            GPIB3458AMeasure m3458A = dataContext.Measure3458AInstance;
            SerialPorts serialPorts = dataContext.SerialPortsInstance;
            DataStorage dataStorage = dataContext.DataStorageInstance;
            
            // 在此处编写代码
            MessageBox.Show("Automation");
            
            return 0;
        }
        catch (Exception ex)
        {
            _ = MessageBox.Show(ex.ToString());
            return -1;
        }
    }
}
