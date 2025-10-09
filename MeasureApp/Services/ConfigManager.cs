using CarrotLink.Core.Utility;
using MeasureApp.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace MeasureApp.Services
{
    public class AppConfig
    {
        public string Version { get; set; } = "1";

        public IEnumerable<PresetCommandItem> PresetCommands { get; set; }
    }

    public class ConfigManager
    {
        private static readonly string appConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appconfig.json");

        public AppConfig AppConfig { get; set; }

        public ConfigManager()
        {
            try
            {
                if (File.Exists(appConfigPath))
                {
                    try
                    {
                        AppConfig = SerializationHelper.DeserializeFromFile<AppConfig>(appConfigPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"获取默认app配置失败, 请检查配置文件:{appConfigPath}\r\n{ex}");
                        File.Move(appConfigPath, appConfigPath + ".old", overwrite: true);
                    }
                }
                if (AppConfig == null)
                {
                    AppConfig = new AppConfig();
                    Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Update()
        {
            try
            {
                SerializationHelper.SerializeToFile(AppConfig, appConfigPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存默认app配置失败\r\n{ex}");
            }
        }
    }
}