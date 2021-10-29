using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
        // 串口指令发送模块文本框前景色绑定
        private SolidColorBrush serialPortSendCmdCommandNameManualTextBoxForeGround = new(Colors.Black);
        public SolidColorBrush SerialPortSendCmdCommandNameManualTextBoxForeGround
        {
            get => serialPortSendCmdCommandNameManualTextBoxForeGround;
            set
            {
                serialPortSendCmdCommandNameManualTextBoxForeGround = value;
                RaisePropertyChanged(() => SerialPortSendCmdCommandNameManualTextBoxForeGround);
            }
        }

        private SolidColorBrush serialPortSendCmdParamsTextBoxForeGround = new(Colors.Black);
        public SolidColorBrush SerialPortSendCmdParamsTextBoxForeGround
        {
            get => serialPortSendCmdParamsTextBoxForeGround;
            set
            {
                serialPortSendCmdParamsTextBoxForeGround = value;
                RaisePropertyChanged(() => SerialPortSendCmdParamsTextBoxForeGround);
            }
        }

        // 串口指令发送模块串口选择数据绑定
        private string serialPortSendCmdSerialPortNameSelectedValue;
        public string SerialPortSendCmdSerialPortNameSelectedValue
        {
            get => serialPortSendCmdSerialPortNameSelectedValue;
            set
            {
                serialPortSendCmdSerialPortNameSelectedValue = value;
                RaisePropertyChanged(() => SerialPortSendCmdSerialPortNameSelectedValue);
            }
        }

        // 串口指令发送模块命令选择数据绑定
        private string serialportSendCmdCommandNameSelectedValue = "SET";
        public string SerialportSendCmdCommandNameSelectedValue
        {
            get => serialportSendCmdCommandNameSelectedValue;
            set
            {
                serialportSendCmdCommandNameSelectedValue = value;
                RaisePropertyChanged(() => SerialportSendCmdCommandNameSelectedValue);

            }
        }

        private string serialPortSendCmdCommandNameManualText = "MANUAL";
        public string SerialPortSendCmdCommandNameManualText
        {
            get => serialPortSendCmdCommandNameManualText;
            set
            {
                serialPortSendCmdCommandNameManualText = value;
                RaisePropertyChanged(() => SerialPortSendCmdCommandNameManualText);
            }
        }



        // 串口指令发送模块参数文本框数据绑定
        private string serialPortSendCmdParamsText;
        public string SerialPortSendCmdParamsText
        {
            get => serialPortSendCmdParamsText;
            set
            {
                serialPortSendCmdParamsText = value;
                RaisePropertyChanged(() => SerialPortSendCmdParamsText);
            }
        }

        // 串口指令发送模块参数文本框数据绑定
        private string serialPortSendCmdPreviewText;
        public string SerialPortSendCmdPreviewText
        {
            get => serialPortSendCmdPreviewText;
            set
            {
                serialPortSendCmdPreviewText = value;
                RaisePropertyChanged(() => SerialPortSendCmdPreviewText);
            }
        }

        // 串口指令发送模块命令编辑事件
        private CommandBase serialPortSendCmdChangedEvent;
        public CommandBase SerialPortSendCmdChangedEvent
        {
            get
            {
                if (serialPortSendCmdChangedEvent == null)
                {
                    serialPortSendCmdChangedEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            string serialPortName = SerialPortSendCmdSerialPortNameSelectedValue ?? "";
                            string commandName = SerialportSendCmdCommandNameSelectedValue == "Others" ? SerialPortSendCmdCommandNameManualText : SerialportSendCmdCommandNameSelectedValue;
                            string[] CommandElements = (commandName + ";" + SerialPortSendCmdParamsText ?? "").Split(" ,.;|&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            SerialPortSendCmdPreviewText = $"{serialPortName}::{string.Join(";", CommandElements)};";

                            SerialPortSendCmdCommandNameManualTextBoxForeGround = new SolidColorBrush(Regex.IsMatch(SerialPortSendCmdCommandNameManualText ?? "", @"[^a-zA-Z0-9]") ? Colors.Red : Colors.Black);
                            SerialPortSendCmdParamsTextBoxForeGround = new SolidColorBrush(Regex.IsMatch(SerialPortSendCmdParamsText ?? "", @"[^x00-xff\s,.;|&]") ? Colors.Red : Colors.Black);
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return serialPortSendCmdChangedEvent;
            }
        }

        // 串口指令发送模块发送事件
        private CommandBase serialPortSendCmdEvent;
        public CommandBase SerialPortSendCmdEvent
        {
            get
            {
                if (serialPortSendCmdEvent == null)
                {
                    serialPortSendCmdEvent = new CommandBase(new Action<object>(param =>
                    {
                        try
                        {
                            string[] splitCmds = SerialPortSendCmdPreviewText.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                            SerialPortsInstance.WriteString(splitCmds[0], splitCmds[1]);
                        }
                        catch (Exception ex)
                        {
                            _ = MessageBox.Show(ex.ToString());
                        }
                    }));
                }
                return serialPortSendCmdEvent;
            }
        }

        // 串口数据接收模块串口选择数据绑定
        private string serialPortRecvDataSerialPortNameSelectedValue;
        public string SerialPortRecvDataSerialPortNameSelectedValue
        {
            get => serialPortRecvDataSerialPortNameSelectedValue;
            set
            {
                serialPortRecvDataSerialPortNameSelectedValue = value;
                RaisePropertyChanged(() => SerialPortRecvDataSerialPortNameSelectedValue);
            }
        }

        // DataGrid数据绑定
        public dynamic SerialPortStorageDataGridBinding
        {
            get => DataStorageInstance.DataStorageDictionary[KeySerialPortString];
            set
            {
                DataStorageInstance.DataStorageDictionary[KeySerialPortString] = value;
                RaisePropertyChanged(() => SerialPortStorageDataGridBinding);
            }
        }
    }
}
