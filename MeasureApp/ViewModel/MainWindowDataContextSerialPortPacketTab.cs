using MeasureApp.Model.Common;
using MeasureApp.Model.Converter;
using MeasureApp.ViewModel.Common;
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
        // 串口选择
        private string spPktCommTabSerialPortName;
        public string SpPktCommTabSerialPortName
        {
            get => spPktCommTabSerialPortName;
            set
            {
                spPktCommTabSerialPortName = value;
                RaisePropertyChanged(() => SpPktCommTabSerialPortName);
            }
        }

        // 字长
        private int spPktCommTabWordLen = 4;
        public int SpPktCommTabWordLen
        {
            get => spPktCommTabWordLen;
            set
            {
                spPktCommTabWordLen = value;
                RaisePropertyChanged(() => SpPktCommTabWordLen);
            }
        }

        // 数据包长
        private int spPktCommTabPktLen = 256;
        public int SpPktCommTabPktLen
        {
            get => spPktCommTabPktLen;
            set
            {
                spPktCommTabPktLen = value;
                RaisePropertyChanged(() => SpPktCommTabPktLen);
            }
        }

        // 数据解析类型
        private string[] pktCvtTypes = new[]{
            "System.UInt16",
            "System.UInt32",
            "System.String",
        };
        public string[] PktCvtTypes
        {
            get => pktCvtTypes;
        }

        private string spPktCommTabSelectedPktCvtType;
        public string SpPktCommTabSelectedPktCvtType
        {
            get => spPktCommTabSelectedPktCvtType;
            set
            {
                spPktCommTabSelectedPktCvtType = value;
                RaisePropertyChanged(() => SpPktCommTabSelectedPktCvtType);
            }
        }


        // 读取数据包
        public string SpPktCommTabReadPkt(string command, int wordLen, int pktLen)
        {
            byte[] pktData = SerialPortsInstance.GetDataPacket(SpPktCommTabSerialPortName, command, wordLen * pktLen);
            return BytesConverter.Bytes2Hex(pktData);
        }

        public T[] SpPktCommTabReadPkt<T>(string command, int wordLen, int pktLen) where T : struct
        {
            string data = SpPktCommTabReadPkt(command, wordLen, pktLen);
            return BytesConverter.FromBytes<T>(data);
        }

        public object SpPktCommTabReadPkt(string command, int wordLen, int pktLen, Type type)
        {
            if (type == typeof(string))
            {
                string bytesPacket = SpPktCommTabReadPkt(command, wordLen, pktLen);
                string[] bytes = new string[bytesPacket.Length / SpPktCommTabWordLen / 2];
                for (int i = 0; i < bytes.Length ; i += 1)
                    bytes[i] = bytesPacket.Substring(2 * i * SpPktCommTabWordLen, 2 * SpPktCommTabWordLen);
                return bytes;
            }
            else
                return this.GetType()
                .GetMethod(nameof(SpPktCommTabReadPkt), 1, new Type[] { typeof(string), typeof(int), typeof(int) })
                .MakeGenericMethod(type)
                .Invoke(this, new object[] { command, wordLen, pktLen });
        }

        public object SpPktCommTabReadPkt(string command, int wordLen, int pktLen, string type)
        {
            return SpPktCommTabReadPkt(command, wordLen, pktLen, Type.GetType(type));
        }

        public void SpPktCommTabReadPktGUI(object param)
        {
            try
            {
                string command;
                int pktLen;
                if ((string)param == "word")
                {
                    command = "PKT;0;";
                    pktLen = 1;
                }
                else
                {
                    command = "PKT;1;";
                    pktLen = SpPktCommTabPktLen;
                }
                object dat = SpPktCommTabReadPkt(command, SpPktCommTabWordLen, pktLen, SpPktCommTabSelectedPktCvtType);
                DataStorageInstance.AddData(KeySerialPortString, dat);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // CommandBase
        private CommandBase spPktCommTabReadPktEvent;
        public CommandBase SpPktCommTabReadPktEvent
        {
            get
            {
                if (spPktCommTabReadPktEvent == null)
                {
                    spPktCommTabReadPktEvent = new CommandBase(new Action<object>(param => SpPktCommTabReadPktGUI(param)));
                }
                return spPktCommTabReadPktEvent;
            }
        }

    }
}
