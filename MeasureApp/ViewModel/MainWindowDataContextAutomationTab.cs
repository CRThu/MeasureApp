using ICSharpCode.AvalonEdit.Document;
using MeasureApp.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        private static string automationCodeEditorDefaultText = @"/// lang=C#
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
    	string Key3458AString = ""3458A Data Storage"";

        GPIB3458AMeasure m3458A = (dataContext as MainWindowDataContext).Measure3458AInstance;
        SerialPorts serialPorts = (dataContext as MainWindowDataContext).SerialPortsInstance;
        DataStorage dataStorage = (dataContext as MainWindowDataContext).DataStorageInstance;
        
        
       	string com = serialPorts.SerialPortNames.First();
		int delay = dataContext.DelayText;
		int LoopTimes = dataContext.LoopTimesText;
		decimal M3458ARange = dataContext.MultiMeterSetRangeText;
		decimal M3458AResolution = dataContext.MultiMeterSetResolutionText / 1e6M;
		byte[] SendCommandByteText = Utility.ToBytesFromHexString(dataContext.SendCommandByteText);
		decimal voltage;
        
       	if (!m3458A.IsOpen)
		{
			throw new NullReferenceException(""3458A未打开"");
		}

		// 丢弃现有缓存
		_ = m3458A.ReadDecimal();
		
		// 采集过程
		for (int i = 0; i < LoopTimes; i++)
		{
			// 向DAC下位机发送电压命令
			dataContext.StatusBarText = $""{i + 1}A/{LoopTimes}"";
			serialPorts.WriteBytes(com, SendCommandByteText, SendCommandByteText.Length);
			
			// 等待delay拍数，期间采集的数据丢弃
			dataContext.StatusBarText = $""{i + 1}B/{LoopTimes}"";
			for (int j = 0; j < delay; j++)
			{
				_ = m3458A.ReadDecimal();
			}
			
			// 从3458A接收自动采集的电压命令
			dataContext.StatusBarText = $""{i + 1}C/{LoopTimes}"";
			voltage = m3458A.ReadDecimal();
			
			// 存储电压
			dataContext.StatusBarText = $""{i + 1}D/{LoopTimes}"";
			dataStorage.AddData(Key3458AString, voltage);
		}
        return 0;
    }
}";

        // 自动化程序编辑器代码绑定
        private string automationCodeEditorText = automationCodeEditorDefaultText;
        public string AutomationCodeEditorText
        {
            get => automationCodeEditorText;
            set
            {
                automationCodeEditorText = value;
                RaisePropertyChanged(() => AutomationCodeEditorText);
            }
        }


        // 自动化程序执行事件
        private CommandBase automationCodeRunEvent;
        public CommandBase AutomationCodeRunEvent
        {
            get
            {
                if (automationCodeRunEvent == null)
                {
                    automationCodeRunEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            _ = Task.Run(() =>
                            {
                                try
                                {
                                    string code = AutomationCodeEditorText;
                                    var type = CodeCompiler.Run(code, "Automation");
                                    var transformer = Activator.CreateInstance(type);
                                    var newContent = type.GetMethod("Main").Invoke(transformer, new object[] { this });
                                    //MessageBox.Show($"result={newContent}");
                                    AutomationCodeReturnData = newContent.ToString();
                                }
                                catch (Exception ex)
                                {
                                    _ = MessageBox.Show(ex.ToString());
                                }
                            });
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return automationCodeRunEvent;
            }
        }

        // 自动化程序运行返回值
        private string automationCodeReturnData;
        public string AutomationCodeReturnData
        {
            get => automationCodeReturnData;
            set
            {
                automationCodeReturnData = value;
                RaisePropertyChanged(() => AutomationCodeReturnData);
            }
        }

        // 临时发送DAC命令数据绑定
        private int loopTimesText = 262144;
        public int LoopTimesText
        {
            get => loopTimesText;
            set
            {
                loopTimesText = value;
                RaisePropertyChanged(() => LoopTimesText);
            }
        }

        private string sendCommandByteText = "02A600";
        public string SendCommandByteText
        {
            get => sendCommandByteText;
            set
            {
                sendCommandByteText = value;
                RaisePropertyChanged(() => SendCommandByteText);
            }
        }

        private decimal multiMeterSetRangeText = 10M;
        public decimal MultiMeterSetRangeText
        {
            get => multiMeterSetRangeText;
            set
            {
                multiMeterSetRangeText = value;
                RaisePropertyChanged(() => MultiMeterSetRangeText);
            }
        }

        private decimal multiMeterSetResolutionText = 1M;
        public decimal MultiMeterSetResolutionText
        {
            get => multiMeterSetResolutionText;
            set
            {
                multiMeterSetResolutionText = value;
                RaisePropertyChanged(() => MultiMeterSetResolutionText);
            }
        }

        private int delayText = 2;
        public int DelayText
        {
            get => delayText;
            set
            {
                delayText = value;
                RaisePropertyChanged(() => DelayText);
            }
        }
    }
}
