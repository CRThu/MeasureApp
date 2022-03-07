using MeasureApp.Model.Common;
using MeasureApp.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.Model
{
    public enum TaskRunStatus
    {
        Waiting,
        Running,
        Finished,
        Error
    }

    public class RunTaskItem : NotificationObjectBase
    {
        private int id;
        public int Id
        {
            get => id;
            set
            {
                id = value;
                RaisePropertyChanged(() => Id);
            }
        }

        private string description;
        public string Description
        {
            get => description;
            set
            {
                description = value;
                RaisePropertyChanged(() => Description);
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

        private TaskRunStatus status;
        public TaskRunStatus Status
        {
            get => status;
            set
            {
                status = value;
                RaisePropertyChanged(() => Status);
            }
        }

        private double progressPercent;
        public double ProgressPercent
        {
            get => progressPercent;
            set
            {
                progressPercent = value;
                RaisePropertyChanged(() => ProgressPercent);
            }
        }

        public delegate string TaskFuncDelegate(string obj, TaskProgressDelegate progressFunc);
        public delegate void TaskProgressDelegate(double percent);
        public TaskFuncDelegate Func;
        public TaskFuncDelegate ResultProcFunc;

        private void ProgressUpdate(double percent)
        {
            ProgressPercent = percent;
        }

        private Task InvokeGeneralFunc(TaskFuncDelegate f, string p, bool isUpdateRetVal)
        {
            return Task.Run(() =>
            {
                try
                {
                    Status = TaskRunStatus.Running;
                    string ret = f.Invoke(p, ProgressUpdate);
                    if (isUpdateRetVal)
                        ReturnVal = ret;
                    Status = TaskRunStatus.Finished;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    Status = TaskRunStatus.Error;
                }
            });
        }

        public Task InvokeTaskFunc()
        {
            return InvokeGeneralFunc(Func, ParamVal, true);
        }

        public Task InvokeResultProcFunc()
        {
            return InvokeGeneralFunc(ResultProcFunc, ReturnVal, false);
        }

        private CommandBase runTaskEvent;
        public CommandBase RunTaskEvent
        {
            get
            {
                if (runTaskEvent == null)
                {
                    runTaskEvent = new CommandBase(new Action<object>(param => InvokeTaskFunc()));
                }
                return runTaskEvent;
            }
        }

        private CommandBase runTaskResultProcEvent;
        public CommandBase RunTaskResultProcEvent
        {
            get
            {
                if (runTaskResultProcEvent == null)
                {
                    runTaskResultProcEvent = new CommandBase(new Action<object>(param => InvokeResultProcFunc()));
                }
                return runTaskResultProcEvent;
            }
        }


        public static IEnumerable<RunTaskItem> ConvertClassToRunTaskItems(Type classType)
        {
            TaskFuncDelegate resultProcFunc = TaskMethodInfoAttribute.GetMethodsContainAttribute(classType)
                .Where(m => m.GetCustomAttribute<TaskMethodInfoAttribute>().IsReturnProc == true)
                .First().CreateDelegate<TaskFuncDelegate>();
            List<RunTaskItem> runTaskItems = new();
            var ms = TaskMethodInfoAttribute.GetMethodsContainAttribute(classType)
                .Where(m => m.GetCustomAttribute<TaskMethodInfoAttribute>().IsReturnProc == false);

            foreach (var m in ms)
            {
                var attr = m.GetCustomAttribute<TaskMethodInfoAttribute>();
                if (attr.Id != null)
                    runTaskItems.Add(new RunTaskItem()
                    {
                        Id = (int)attr.Id,
                        Description = m.Name,
                        Func = m.CreateDelegate<TaskFuncDelegate>(),
                        ResultProcFunc = resultProcFunc
                    });
            }
            return runTaskItems;
        }
    }
}
