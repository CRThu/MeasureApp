using CarrotLink.Core.Protocols.Configuration;
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

        public int RegisterRequestTimeout { get; set; } = 1000;

        public string RegisterMapFilePath { get; set; }
    }

    public class CarrotLinkConfig
    {
        public string Version { get; set; } = "1";

        public CarrotAsciiProtocolConfiguration CarrotAsciiProtocolConfiguration { get; set; } = new()
        {
            RegfilesCommands = new CarrotAsciiProtocolRegfileCommands[]
            {
                new CarrotAsciiProtocolRegfileCommands()
                {
                    Name = "<regfile>",
                    WriteRegCommand = "REG.W",
                    ReadRegCommand = "REG.R",
                    WriteBitsCommand = "REG.BW",
                    ReadBitsCommand = "REG.BR"
                }
            }
        };
    }


    public class ConfigManager
    {
        private static readonly string appConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appconfig.json");
        private static readonly string carrotLinkConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "carrotlinkconfig.json");

        public AppConfig AppConfig { get; set; }
        public CarrotLinkConfig CarrotLinkConfig { get; set; }

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

                if (File.Exists(carrotLinkConfigPath))
                {
                    try
                    {
                        CarrotLinkConfig = SerializationHelper.DeserializeFromFile<CarrotLinkConfig>(carrotLinkConfigPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"获取默认carrotlink配置失败, 请检查配置文件:{carrotLinkConfigPath}\r\n{ex}");
                        File.Move(carrotLinkConfigPath, carrotLinkConfigPath + ".old", overwrite: true);
                    }
                }
                if (CarrotLinkConfig == null)
                {
                    CarrotLinkConfig = new CarrotLinkConfig();
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
            try
            {
                SerializationHelper.SerializeToFile(CarrotLinkConfig, carrotLinkConfigPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存默认carrotlink配置失败\r\n{ex}");
            }
        }
    }
}