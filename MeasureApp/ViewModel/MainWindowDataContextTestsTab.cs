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

namespace MeasureApp.ViewModel
{
    public partial class MainWindowDataContext : NotificationObjectBase
    {
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
        private string nb2005TestTaskResult1;
        public string Nb2005TestTaskResult1
        {
            get => nb2005TestTaskResult1;
            set
            {
                nb2005TestTaskResult1 = value;
                RaisePropertyChanged(() => Nb2005TestTaskResult1);
            }
        }

        private string nb2005TestTaskResult2;
        public string Nb2005TestTaskResult2
        {
            get => nb2005TestTaskResult2;
            set
            {
                nb2005TestTaskResult2 = value;
                RaisePropertyChanged(() => Nb2005TestTaskResult2);
            }
        }

        private string nb2005TestTaskResult3;
        public string Nb2005TestTaskResult3
        {
            get => nb2005TestTaskResult3;
            set
            {
                nb2005TestTaskResult3 = value;
                RaisePropertyChanged(() => Nb2005TestTaskResult3);
            }
        }

        private string nb2005TestTaskResult4;
        public string Nb2005TestTaskResult4
        {
            get => nb2005TestTaskResult4;
            set
            {
                nb2005TestTaskResult4 = value;
                RaisePropertyChanged(() => Nb2005TestTaskResult4);
            }
        }

        // 测试任务实现
        public void Nb2005TestTask1()
        {

        }
        public void Nb2005TestTask2()
        {

        }
        public void Nb2005TestTask3()
        {

        }
        public void Nb2005TestTask4()
        {

        }
    }
}
