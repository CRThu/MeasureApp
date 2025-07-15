using MeasureApp.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.DynamicCompilation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TaskMethodInfoAttribute : Attribute
    {
        /// <summary>
        /// 当方法为普通任务方法类型时, Id表示任务方法显示的顺序, 当为结果处理方法类型, Id应为Null。
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// IsReturnProc在class中设置的IsReturnProc方法只能存在一个, True表示方法为结果处理方法类型, False表示方法为普通任务方法类型。
        /// </summary>
        public bool IsReturnProc { get; set; }

        public TaskMethodInfoAttribute(int id)
        {
            this.Id = id;
        }

        public TaskMethodInfoAttribute(bool returnFunc)
        {
            this.IsReturnProc = returnFunc;
        }

        public static MethodInfo[] GetMethodsContainAttribute(Type classType)
        {
            return CustomAttributeHelper.GetMethodsContainAttribute<TaskMethodInfoAttribute>(classType);
        }
    }
}
