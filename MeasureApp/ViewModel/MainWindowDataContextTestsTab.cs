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
using MeasureApp.ViewModel.Common;
using MeasureApp.Model.Register;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Threading;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 配置文件路径
        private string nb2005ChipsConfigFilePath;
        public string Nb2005ChipsConfigFilePath
        {
            get => nb2005ChipsConfigFilePath;
            set
            {
                nb2005ChipsConfigFilePath = value;
                RaisePropertyChanged(() => Nb2005ChipsConfigFilePath);
            }
        }

        // 芯片ID
        private string nb2005ChipId;
        public string Nb2005ChipId
        {
            get => nb2005ChipId;
            set
            {
                nb2005ChipId = value;
                RaisePropertyChanged(() => Nb2005ChipId);
            }
        }

        // 测试项结果
        private decimal? nb2005TestTaskResult1;
        public decimal? Nb2005TestTaskResult1
        {
            get => nb2005TestTaskResult1;
            set
            {
                nb2005TestTaskResult1 = value;
                RaisePropertyChanged(() => Nb2005TestTaskResult1);
            }
        }

        private decimal? nb2005TestTaskResult2;
        public decimal? Nb2005TestTaskResult2
        {
            get => nb2005TestTaskResult2;
            set
            {
                nb2005TestTaskResult2 = value;
                RaisePropertyChanged(() => Nb2005TestTaskResult2);
            }
        }

        private decimal? nb2005TestTaskResult3;
        public decimal? Nb2005TestTaskResult3
        {
            get => nb2005TestTaskResult3;
            set
            {
                nb2005TestTaskResult3 = value;
                RaisePropertyChanged(() => Nb2005TestTaskResult3);
            }
        }

        private decimal? nb2005TestTaskResult4;
        public decimal? Nb2005TestTaskResult4
        {
            get => nb2005TestTaskResult4;
            set
            {
                nb2005TestTaskResult4 = value;
                RaisePropertyChanged(() => Nb2005TestTaskResult4);
            }
        }

        // 测试任务实现
        private void Nb2005TestTaskSwitch(object param)
        {
            try
            {
                decimal result;
                switch (Convert.ToInt32(param))
                {
                    case 1:
                        result = Nb2005TestTask1();
                        Nb2005TestTaskResult1 = result;
                        break;
                    case 2:
                        result = Nb2005TestTask2();
                        Nb2005TestTaskResult2 = result;
                        break;
                    case 3:
                        result = Nb2005TestTask3();
                        Nb2005TestTaskResult3 = result;
                        break;
                    case 4:
                        result = Nb2005TestTask4();
                        Nb2005TestTaskResult4 = result;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public decimal Nb2005TestTask1()
        {
            // TODO
            return 1M;
        }
        public decimal Nb2005TestTask2()
        {
            // TODO
            return 2M;
        }
        public decimal Nb2005TestTask3()
        {
            // TODO
            return 3M;
        }
        public decimal Nb2005TestTask4()
        {
            // TODO
            return 4M;
        }

        // 测试结果写入
        public void Nb2005TestResultWriteAll(object param)
        {
            try
            {
                int count = Convert.ToInt32(param);
                for (int i = 0; i < count; i++)
                {
                    Nb2005TestResultWrite(i + 1);
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Nb2005TestResultWrite(object param)
        {
            try
            {
                switch (Convert.ToInt32(param))
                {
                    case 1:
                        Nb2005TestTask1ResultWrite(Nb2005TestTaskResult1);
                        break;
                    case 2:
                        Nb2005TestTask2ResultWrite(Nb2005TestTaskResult2);
                        break;
                    case 3:
                        Nb2005TestTask3ResultWrite(Nb2005TestTaskResult3);
                        break;
                    case 4:
                        Nb2005TestTask4ResultWrite(Nb2005TestTaskResult4);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Nb2005TestTask1ResultWrite(decimal? data)
        {
            if (data != null)
            {
                // TODO
                string serialPortName = SerialPortsInstance.SerialPortNames.First();
                SerialPortsInstance.WriteString(serialPortName, $"TASK1.RESULT={data}\n");
            }
            else
            {
                throw new Exception("decimal? data is null.");
            }
        }
        public void Nb2005TestTask2ResultWrite(decimal? data)
        {
            if (data != null)
            {
                // TODO
                string serialPortName = SerialPortsInstance.SerialPortNames.First();
                SerialPortsInstance.WriteString(serialPortName, $"TASK2.RESULT={data}\n");
            }
            else
            {
                throw new Exception("decimal? data is null.");
            }
        }
        public void Nb2005TestTask3ResultWrite(decimal? data)
        {
            if (data != null)
            {
                // TODO
                string serialPortName = SerialPortsInstance.SerialPortNames.First();
                SerialPortsInstance.WriteString(serialPortName, $"TASK3.RESULT={data}\n");
            }
            else
            {
                throw new Exception("decimal? data is null.");
            }
        }
        public void Nb2005TestTask4ResultWrite(decimal? data)
        {
            if (data != null)
            {
                // TODO
                string serialPortName = SerialPortsInstance.SerialPortNames.First();
                SerialPortsInstance.WriteString(serialPortName, $"TASK4.RESULT={data}\n");
            }
            else
            {
                throw new Exception("decimal? data is null.");
            }
        }

        // 配置文件加载
        public void Nb2005LoadChipsConfigFile()
        {
            try
            {
                // Open File Dialog
                OpenFileDialog openFileDialog = new()
                {
                    Title = "Open Json File...",
                    Filter = "Json File|*.json",
                    InitialDirectory = Properties.Settings.Default.DefaultDirectory
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    Properties.Settings.Default.DefaultDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                    Nb2005ChipsConfigFilePath = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // 读取配置
        public void Nb2005LoadChipConfig()
        {
            try
            {
                string chipsTrimInfoJson;
                ChipsTrimInfo chipsTrimInfo = new();
                chipsTrimInfoJson = File.ReadAllText(Nb2005ChipsConfigFilePath);
                chipsTrimInfo = JsonConvert.DeserializeObject<ChipsTrimInfo>(chipsTrimInfoJson, new JsonSerializerSettings() { FloatParseHandling = FloatParseHandling.Decimal });

                Nb2005TestTaskResult1 = chipsTrimInfo.Get(Nb2005ChipId, "TASK1");
                Nb2005TestTaskResult2 = chipsTrimInfo.Get(Nb2005ChipId, "TASK2");
                Nb2005TestTaskResult3 = chipsTrimInfo.Get(Nb2005ChipId, "TASK3");
                Nb2005TestTaskResult4 = chipsTrimInfo.Get(Nb2005ChipId, "TASK4");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // 配置更新到文件
        public void Nb2005UpdateChipConfig()
        {
            try
            {
                // 若存在文件则加载
                string chipsTrimInfoJson;
                ChipsTrimInfo chipsTrimInfo = new();
                if (File.Exists(Nb2005ChipsConfigFilePath))
                {
                    chipsTrimInfoJson = File.ReadAllText(Nb2005ChipsConfigFilePath);
                    chipsTrimInfo = JsonConvert.DeserializeObject<ChipsTrimInfo>(chipsTrimInfoJson, new JsonSerializerSettings() { FloatParseHandling = FloatParseHandling.Decimal });
                }
                else
                {
                    SaveFileDialog saveFileDialog = new()
                    {
                        Title = "保存芯片Trim文件",
                        FileName = $"ChipsTrimInfo.{DataStorage.GenerateDateTimeNow()}.json",
                        DefaultExt = ".json",
                        Filter = "Json File|*.json",
                        InitialDirectory = Properties.Settings.Default.DefaultDirectory
                    };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        Properties.Settings.Default.DefaultDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                        Nb2005ChipsConfigFilePath = saveFileDialog.FileName;
                    }
                    else
                        return;
                }

                chipsTrimInfo.Set(Nb2005ChipId, "TASK1", Nb2005TestTaskResult1);
                chipsTrimInfo.Set(Nb2005ChipId, "TASK2", Nb2005TestTaskResult2);
                chipsTrimInfo.Set(Nb2005ChipId, "TASK3", Nb2005TestTaskResult3);
                chipsTrimInfo.Set(Nb2005ChipId, "TASK4", Nb2005TestTaskResult4);

                chipsTrimInfoJson = JsonConvert.SerializeObject(chipsTrimInfo, new JsonSerializerSettings() { FloatParseHandling = FloatParseHandling.Decimal });
                File.WriteAllText(Nb2005ChipsConfigFilePath, chipsTrimInfoJson);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        // CommandBase
        private CommandBase nb2005TestTaskEvent;
        public CommandBase Nb2005TestTaskEvent
        {
            get
            {
                if (nb2005TestTaskEvent == null)
                {
                    nb2005TestTaskEvent = new CommandBase(new Action<object>(param => Nb2005TestTaskSwitch(param)));
                }
                return nb2005TestTaskEvent;
            }
        }

        private CommandBase nb2005TestResultWriteEvent;
        public CommandBase Nb2005TestResultWriteEvent
        {
            get
            {
                if (nb2005TestResultWriteEvent == null)
                {
                    nb2005TestResultWriteEvent = new CommandBase(new Action<object>(param => Nb2005TestResultWrite(param)));
                }
                return nb2005TestResultWriteEvent;
            }
        }

        private CommandBase nb2005LoadChipsConfigFileEvent;
        public CommandBase Nb2005LoadChipsConfigFileEvent
        {
            get
            {
                if (nb2005LoadChipsConfigFileEvent == null)
                {
                    nb2005LoadChipsConfigFileEvent = new CommandBase(new Action<object>(param => Nb2005LoadChipsConfigFile()));
                }
                return nb2005LoadChipsConfigFileEvent;
            }
        }

        private CommandBase nb2005LoadChipConfigEvent;
        public CommandBase Nb2005LoadChipConfigEvent
        {
            get
            {
                if (nb2005LoadChipConfigEvent == null)
                {
                    nb2005LoadChipConfigEvent = new CommandBase(new Action<object>(param => Nb2005LoadChipConfig()));
                }
                return nb2005LoadChipConfigEvent;
            }
        }

        private CommandBase nb2005UpdateChipConfigEvent;
        public CommandBase Nb2005UpdateChipConfigEvent
        {
            get
            {
                if (nb2005UpdateChipConfigEvent == null)
                {
                    nb2005UpdateChipConfigEvent = new CommandBase(new Action<object>(param => Nb2005UpdateChipConfig()));
                }
                return nb2005UpdateChipConfigEvent;
            }
        }

        private CommandBase nb2005TestResultWriteAllEvent;
        public CommandBase Nb2005TestResultWriteAllEvent
        {
            get
            {
                if (nb2005TestResultWriteAllEvent == null)
                {
                    nb2005TestResultWriteAllEvent = new CommandBase(new Action<object>(param => Nb2005TestResultWriteAll(param)));
                }
                return nb2005TestResultWriteAllEvent;
            }
        }

    }
}
