using MeasureApp.Model.Common;
using MeasureApp.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.Model
{
    public class ClassTestBase : NotificationObjectBase
    {
        private string id;
        public string Id
        {
            get => id;
            set
            {
                id = value;
                RaisePropertyChanged(() => Id);
            }
        }

        private string funcName;
        public string FuncName
        {
            get => funcName;
            set
            {
                funcName = value;
                RaisePropertyChanged(() => FuncName);
            }
        }

        private string paramVal;
        public string ParamVal
        {
            get => paramVal;
            set
            {
                paramVal = value;
                RaisePropertyChanged(() => ParamVal);
            }
        }

        private string returnVal;
        public string ReturnVal
        {
            get => returnVal;
            set
            {
                returnVal = value;
                RaisePropertyChanged(() => ReturnVal);
            }
        }

        public delegate string ClassDelegate(string obj);
        public ClassDelegate Func;

        private void MessageBoxTest(object p)
        {
            ReturnVal = Func?.Invoke(p.ToString());
        }

        private CommandBase messageBoxTestEvent;
        public CommandBase MessageBoxTestEvent
        {
            get
            {
                if (messageBoxTestEvent == null)
                {
                    messageBoxTestEvent = new CommandBase(new Action<object>(param => MessageBoxTest(ParamVal)));
                }
                return messageBoxTestEvent;
            }
        }

    }
}
