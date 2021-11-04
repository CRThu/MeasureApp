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

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
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
