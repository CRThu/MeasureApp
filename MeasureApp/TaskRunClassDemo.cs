using MeasureApp.Model;
using MeasureApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp
{
    public static class TaskRunClassDemo
    {
        public static string FuncX(string p)
        {
            MessageBox.Show("FUNCX CALLED");
            return "FUNCX:" + p;
        }

        [TaskMethodInfo(id: 1)]
        public static string Func1(string p)
        {
            MessageBox.Show("FUNC1 CALLED");
            return "FUNC1:" + p;
        }

        [TaskMethodInfo(id: 2)]
        public static string Func2(string p)
        {
            MessageBox.Show("FUNC2 CALLED");
            return "FUNC2:" + p;
        }

        [TaskMethodInfo(id: 3)]
        public static string Func3(string p)
        {
            MessageBox.Show("FUNC3 CALLED");
            return "FUNC3:" + p;
        }
    }
}
