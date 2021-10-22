using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 3458A基本命令状态文本
        private string m3458ABasicConfigCommandLogText;
        public string M3458ABasicConfigCommandLogText
        {
            get => m3458ABasicConfigCommandLogText;
            set
            {
                m3458ABasicConfigCommandLogText = value;
                RaisePropertyChanged(() => M3458ABasicConfigCommandLogText);
            }
        }

        // 3458A发送命令事件
        private CommandBase m3458ABasicConfigEvent;
        public CommandBase M3458ABasicConfigEvent
        {
            get
            {
                if (m3458ABasicConfigEvent == null)
                {
                    m3458ABasicConfigEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            if (Measure3458AInstance.IsOpen)
                            {
                                switch (param as string)
                                {
                                    case "RESET":
                                        Measure3458AInstance.Reset();
                                        M3458ABasicConfigCommandLogText = $"Write: RESET & END";
                                        break;
                                    case "ID":
                                        M3458ABasicConfigCommandLogText = $"Query: ID?\nReturn: {Measure3458AInstance.GetID()}";
                                        break;
                                    case "ERR":
                                        M3458ABasicConfigCommandLogText = $"Query: ERRSTR?\nReturn: {Measure3458AInstance.GetErrorString()}";
                                        break;
                                    case "STB":
                                        M3458ABasicConfigCommandLogText = $"Query: STB?\nReturn: {Measure3458AInstance.ReadStatusByte()}";
                                        break;
                                    case "TEMP":
                                        M3458ABasicConfigCommandLogText = $"Query: TEMP?\nReturn: {Measure3458AInstance.GetTemp()}";
                                        break;
                                    case "LINE":
                                        M3458ABasicConfigCommandLogText = $"Query: LINE?\nReturn: {Measure3458AInstance.GetLineFreq()} Hz";
                                        break;
                                    // TODO
                                    //case "NDIG":
                                    //    string setNdig = (NdigComboBox.SelectedItem as ComboBoxItem).Tag as string;
                                    //    Measure3458AInstance.WriteCommand($"NDIG {setNdig}");
                                    //    M3458ABasicConfigCommandLogText = $"Write: NDIG {setNdig}";
                                    //    break;
                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                            else
                            {
                                _ = MessageBox.Show("GPIB(3458A) is not open.");
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return m3458ABasicConfigEvent;
            }
        }
    }
}
