using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.Common
{
    /// <summary>
    /// 程序配置类
    /// </summary>
    public class AppConfig
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
        public static AppConfig Read()
        {
            return Read(DefaultAppConfigFilePath);
        }

        /// <summary>
        /// 读取配置文件,若不存在则新建默认配置传入
        /// </summary>
        /// <param name="configFilePath">配置文件路径</param>
        /// <returns>返回程序配置类</returns>
        public static AppConfig Read(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                return Json.DeSerializeFromFile<AppConfig>(configFilePath);
            }
            else
            {
                AppConfig appConfig = new AppConfig();
                Json.SerializeToFile(appConfig, configFilePath);
                return appConfig;
            }
        }

        /// <summary>
        /// 更新配置文件
        /// </summary>
        /// <param name="appConfig">程序配置类</param>
        public void Update()
        {
            Update(DefaultAppConfigFilePath);
        }

        /// <summary>
        /// 更新配置文件
        /// </summary>
        /// <param name="appConfig">程序配置类</param>
        public void Update(string configFilePath)
        {
            Json.SerializeToFile(this, configFilePath);
        }
    }

    public class GeneralSettings
    {
        public string DefaultDirectory { get; set; } = "./";
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
            public int DefaultStopBits { get; set; } = 1;
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
            public double[] StopBits { get; set; } = new double[]
            {
                1,
                1.5,
                2
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
