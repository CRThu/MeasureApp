using MeasureApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 串口选择
        private string serialportCommandPortNameSelectedValue;
        public string SerialportCommandPortNameSelectedValue
        {
            get => serialportCommandPortNameSelectedValue;
            set
            {
                serialportCommandPortNameSelectedValue = value;
                RaisePropertyChanged(() => SerialportCommandPortNameSelectedValue);
            }
        }


        // GUI绑定
        private ObservableCollection<SerialportCommandModel> serialportCommandModels = new()
        {
            new SerialportCommandModel("写字节", 1, "DATW"),
            new SerialportCommandModel("读字节", 0, "DATR"),
            new SerialportCommandModel("写寄存器", 2, "REGW"),
            new SerialportCommandModel("读寄存器", 1, "REGR"),
            new SerialportCommandModel("写寄存器位", 3, "REGM"),
            new SerialportCommandModel("读寄存器位", 2, "REGQ"),
        };

        public ObservableCollection<SerialportCommandModel> SerialportCommandModels
        {
            get => serialportCommandModels;
            set
            {
                serialportCommandModels = value;
                RaisePropertyChanged(() => SerialportCommandModels);
            }
        }

        private string commandLog;
        public string CommandLog
        {
            get => commandLog;
            set
            {
                commandLog = value;
                RaisePropertyChanged(() => CommandLog);
            }
        }

        // 发送指令事件
        private CommandBase serialportCommandSendEvent;
        public CommandBase SerialportCommandSendEvent
        {
            get
            {
                if (serialportCommandSendEvent == null)
                {
                    serialportCommandSendEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            if (param is string)
                            {
                                int index = Convert.ToInt32(param);
                                // TODO FIXED PORTNAME
                                string com = SerialPortsInstance.SerialPortNames.First();
                                List<string> vs = new(SerialportCommandModels[index].ParamTexts);
                                vs.Insert(0, SerialportCommandModels[index].CommandText);
                                SerialPortsInstance.WriteString(com, $"{string.Join(";", vs)};");
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return serialportCommandSendEvent;
            }
        }

        // 串口监听回调
        private void SerialPortDataReceivedCallBack(object sender, SerialDataReceivedEventArgs e)
        {
            int _bytesToRead = SerialPortsInstance.SerialPortInstances.First().BytesToRead;
            if (_bytesToRead > 0)
            {
                CommandLog += SerialPortsInstance.ReadExistingString(SerialPortsInstance.SerialPortNames.First()) + "\n";
            }
        }
    }
}
