using MeasureApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeasureApp.Model.Common;

namespace MeasureApp.Model
{
    public class SerialportCommandModel : NotificationObjectBase
    {
        public Func<string[], dynamic[]> sendParamsProc = (p) => { return p; };
        public SerialportCommandModel(string commandInfo, int paramNum, string commandText)
        {
            CommandInfo = commandInfo;
            paramTexts = new string[paramNum];
            CommandText = commandText;
        }

        public SerialportCommandModel(string commandInfo, int paramNum, string commandText, Func<string[], dynamic[]> func)
        {
            CommandInfo = commandInfo;
            paramTexts = new string[paramNum];
            CommandText = commandText;
            sendParamsProc = func;
        }

        // 命令名称
        private string commandInfo;
        public string CommandInfo
        {
            get => commandInfo;
            set
            {
                commandInfo = value;
                RaisePropertyChanged(() => CommandInfo);
            }
        }

        // 命令名称
        private string commandText;
        public string CommandText
        {
            get => commandText;
            set
            {
                commandText = value;
                RaisePropertyChanged(() => CommandText);
            }
        }

        // 参数列表
        private string[] paramTexts;
        public string[] ParamTexts
        {
            get => paramTexts;
            set
            {
                paramTexts = value;
                RaisePropertyChanged(() => ParamTexts);
            }
        }

        // 传出参数
        public dynamic[] SendParamsTexts
        {
            get => sendParamsProc(ParamTexts);
        }
    }
}
