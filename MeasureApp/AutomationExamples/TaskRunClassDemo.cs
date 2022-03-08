using MeasureApp.Model;
using MeasureApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static MeasureApp.Model.RunTaskItem;

//namespace MeasureApp
//{
    public static class TaskRunClassDemo
    {
        public static string FuncX(string p, TaskProgressDelegate progressFunc)
        {
            MessageBox.Show("FUNCX CALLED");
            return "FUNCX:" + p;
        }

        [TaskMethodInfo(id: 1)]
        public static string Func1(string p, TaskProgressDelegate progressFunc)
        {
            MessageBox.Show("FUNC1 CALLED");
            return "FUNC1:" + p;
        }

        [TaskMethodInfo(id: 2)]
        public static string Func2(string p, TaskProgressDelegate progressFunc)
        {
            MessageBox.Show("FUNC2 CALLED");
            return "FUNC2:" + p;
        }

        [TaskMethodInfo(id: 3)]
        public static string Func3(string p, TaskProgressDelegate progressFunc)
        {
            MessageBox.Show("FUNC3 CALLED");
            return "FUNC3:" + p;
        }

        [TaskMethodInfo(id: 4)]
        public static string FuncWait(string p, TaskProgressDelegate progressFunc)
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(50);
                progressFunc((i + 1)/100.0);
            }
            MessageBox.Show("FUNCW CALLED");
            return "FUNCW:" + p;
        }

        [TaskMethodInfo(returnFunc: true)]
        public static string FuncResultProc(string retVal, TaskProgressDelegate progressFunc)
        {
            Thread.Sleep(500);
            Debug.WriteLine($"FuncResultProc():{retVal}.");
            return null;
        }
    }
//}
