using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DryIoc;
using MeasureApp.Messages;
using MeasureApp.Model.Common;
using MeasureApp.Model.Devices;
using MeasureApp.Services;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.ViewModel
{
    public partial class AboutAppVM : BaseVM
    {
        private readonly AppContextManager _context;
        public AppContextManager Context => _context;

        [ObservableProperty]
        private StringBuilder versionText = GetVersion();

        public AboutAppVM(AppContextManager context)
        {
            _context = context;
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

                //stringBuilder.AppendLine();
                //stringBuilder.Append($"COM Device:\t");
                //stringBuilder.AppendJoin("\n\t\t", HardwareInfoUtil.GetSerialPortFullName());
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
            return stringBuilder;
        }
    }
}
