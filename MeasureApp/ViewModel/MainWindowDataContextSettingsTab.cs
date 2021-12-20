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

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 设置
        public int GPIBTimeoutSetting
        {
            get => Properties.Settings.Default.GPIBTimeout;
            set
            {
                Properties.Settings.Default.GPIBTimeout = value;
                RaisePropertyChanged(() => GPIBTimeoutSetting);
            }
        }
        public int SerialPortTimeoutSetting
        {
            get => Properties.Settings.Default.SerialPortReadTimeout;
            set
            {
                Properties.Settings.Default.SerialPortReadTimeout = value;
                Properties.Settings.Default.SerialPortWriteTimeout = value;
                RaisePropertyChanged(() => SerialPortTimeoutSetting);
            }
        }

        public int SerialPortBufferSetting
        {
            get => Properties.Settings.Default.SerialPortReadBufferSize;
            set
            {
                Properties.Settings.Default.SerialPortReadBufferSize = value;
                Properties.Settings.Default.SerialPortWriteBufferSize = value;
                RaisePropertyChanged(() => SerialPortBufferSetting);
            }
        }

        public bool SerialPortLogKeywordHighlightSetting
        {
            get => Properties.Settings.Default.IsSerialPortLogKeywordHighlight;
            set
            {
                Properties.Settings.Default.IsSerialPortLogKeywordHighlight = value;
                RaisePropertyChanged(() => SerialPortLogKeywordHighlightSetting);
            }
        }
        

        // 版本信息
        private StringBuilder settingsVersionText = GetVersion();
        public StringBuilder SettingsVersionText
        {
            get => settingsVersionText;
            set
            {
                settingsVersionText = value;
                RaisePropertyChanged(() => SettingsVersionText);
            }
        }

        public static StringBuilder GetVersion()
        {
            StringBuilder stringBuilder = new();
            try
            {
                var assembly = Assembly.GetEntryAssembly();
                if (assembly == null)
                    assembly = new StackTrace().GetFrames().Last().GetMethod().Module.Assembly;
                var debuggableAttribute = assembly.GetCustomAttribute<DebuggableAttribute>();
                bool isDebugMode = debuggableAttribute.DebuggingFlags.HasFlag(DebuggableAttribute.DebuggingModes.EnableEditAndContinue);

                stringBuilder.AppendLine($"App Version:\t{assembly.GetName().Version}");
                stringBuilder.AppendLine($"Build Time:\t{File.GetLastWriteTime(assembly.Location)}");
                stringBuilder.AppendLine($"Build Mode:\t{(isDebugMode ? "Debug" : "Release")}");
                stringBuilder.AppendLine($".NET Version:\t{Environment.Version}");

                stringBuilder.Append($"GPIB Device:\t");
                stringBuilder.AppendJoin("\n\t\t", GPIB.SearchDevices());

                stringBuilder.AppendLine();
                stringBuilder.Append($"COM Device:\t");
                stringBuilder.AppendJoin("\n\t\t", HardwareInfoUtil.GetSerialPortFullName());
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
            return stringBuilder;
        }
    }
}
