using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MeasureApp.Model.Common
{
    /// <summary>
    /// 程序配置类
    /// </summary>
    public class OldAppConfig
    {
        public GeneralSettings General { get; set; } = new();
        public DeviceSettings Device { get; set; } = new();
        public LoggerSettings Logger { get; set; } = new();
        public TempControlSettings TempControl { get; set; } = new();


        /// <summary>
        /// 默认配置文件路径
        /// </summary>
        public static string DefaultAppConfigFilePath = "./Config/AppConfig.json";
        //public static string DefaultAppConfigFilePath = "./Config/Setting.json";


        /// <summary>
        /// 读取配置文件,若不存在则新建默认配置传入
        /// </summary>
        /// <returns>返回程序配置类</returns>
        public static OldAppConfig Read()
        {
            return Read(DefaultAppConfigFilePath);
        }

        /// <summary>
        /// 读取配置文件,若不存在则新建默认配置传入
        /// </summary>
        /// <param name="configFilePath">配置文件路径</param>
        /// <returns>返回程序配置类</returns>
        public static OldAppConfig Read(string configFilePath)
        {
            try
            {
                OldAppConfig appConfig;
                if (File.Exists(configFilePath))
                {
                    appConfig = Json.DeSerializeFromFile<OldAppConfig>(configFilePath);
                }
                else
                {
                    appConfig = new OldAppConfig();
                    Json.SerializeToFile(appConfig, configFilePath);
                }
                CheckPath(appConfig);
                return appConfig;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return new OldAppConfig();
            }
        }

        /// <summary>
        /// 更新配置文件
        /// </summary>
        public void Update()
        {
            Update(DefaultAppConfigFilePath);
        }

        /// <summary>
        /// 更新配置文件
        /// </summary>
        public void Update(string configFilePath)
        {
            try
            {
                Json.SerializeToFile(this, configFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 检查默认路径是否存在
        /// </summary>
        /// <param name="appConfig"></param>
        public static void CheckPath(OldAppConfig appConfig)
        {
            if ((!Directory.Exists(appConfig.General.DefaultDirectory)) && (appConfig.General.DefaultDirectory != ""))
                appConfig.General.DefaultDirectory = "";
        }
    }

    public class GeneralSettings
    {
        public string DefaultDirectory { get; set; } = "";
        public string DefaultPresetCommandsJsonPath { get; set; } = "./Config/AD7124-8.json";
    }

    public class DeviceSettings
    {
        public VISASettings VISA { get; set; } = new();
        public SerialPortSettings SerialPort { get; set; } = new();


        public class VISASettings
        {
            public int Timeout { get; set; } = 5000;
        };

        public class SerialPortSettings
        {
            public int Timeout { get; set; } = 1000;
            public int Buffer { get; set; } = 65536;
            public int DefaultBaudRate { get; set; } = 9600;
            public string DefaultParity { get; set; } = "None";
            public int DefaultDataBits { get; set; } = 8;
            public float DefaultStopBits { get; set; } = 1;
            public int[] BaudRate { get; set; } = new int[]
            {
                9600,
                38400,
                115200,
                921600
            };
            public string[] Parity { get; set; } = new string[]
            {
                "None",
                "Odd",
                "Even",
                "Mark",
                "Space"
            };
            public int[] DataBits { get; set; } = new int[]
            {
                5,
                6,
                7,
                8
            };
            public float[] StopBits { get; set; } = new float[]
            {
                1.0f,
                1.5f,
                2.0f
            };
        }
    };

    public class LoggerSettings
    {
        public bool IsHighLight { get; set; } = true;
        public Dictionary<string, string> KeywordColor { get; set; } =
        new Dictionary<string, string>{
                { "PASS", "#FF008000" },
                { "FAIL", "#FFFF0000"},
                { "__DEFAULT__", "#FFFFA500"}
        };
    }

    public class TempControlSettings
    {
        public int WarningMinTemp { get; set; } = -55;
        public int WarningMaxTemp { get; set; } = 125;
    }
}
