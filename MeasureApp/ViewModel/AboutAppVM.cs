using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DryIoc;
using MeasureApp.Messages;
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
        private string appVersion = GetMeasureAppVersion();

        [ObservableProperty]
        private string carrotlinkVersion = GetCarrotLinkVersion();

        [ObservableProperty]
        private string appBuildMode = GetAppBuildMode();

        [ObservableProperty]
        private string appBuildTime = GetAppBuildTime();

        [ObservableProperty]
        private string dotnetVersion = GetDotnetVersion();

        public AboutAppVM(AppContextManager context)
        {
            Title = "关于";
            ContentId = "AboutApp";
            _context = context;
        }

        public static string GetMeasureAppVersion()
        {
            string appVersion = "<unknown>";
            try
            {
                var measureAppAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "MeasureApp");
                if (measureAppAssembly != null)
                {
                    appVersion = measureAppAssembly.GetName().Version.ToString();
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
            return appVersion;
        }

        public static string GetCarrotLinkVersion()
        {
            string carrotLinkVersion = "<unknown>";
            try
            {
                var carrotLinkAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "CarrotLink.Core");
                if (carrotLinkAssembly != null)
                {
                    carrotLinkVersion = carrotLinkAssembly.GetName().Version.ToString();
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
            return carrotLinkVersion;
        }

        public static string GetAppBuildMode()
        {
            string buildMode = "<unknown>";
            try
            {
                var assembly = Assembly.GetEntryAssembly();
                var debuggableAttribute = assembly.GetCustomAttribute<DebuggableAttribute>();
                bool isDebugMode = debuggableAttribute.DebuggingFlags.HasFlag(DebuggableAttribute.DebuggingModes.EnableEditAndContinue);
                buildMode = (isDebugMode ? "Debug" : "Release");
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
            return buildMode;
        }

        public static string GetAppBuildTime()
        {
            string buildTime = "<unknown>";
            try
            {
                var assembly = Assembly.GetEntryAssembly();
                buildTime = File.GetLastWriteTime(assembly.Location).ToString();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
            return buildTime;
        }

        public static string GetDotnetVersion()
        {
            return Environment.Version.ToString();
        }
    }
}
