using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.Common
{
    public static class CustomAttributeHelper
    {
        public static MethodInfo[] GetMethodsContainAttribute<T>(Type classType)
        {
            return classType.GetMethods().Where(x => x.IsDefined(typeof(T))).ToArray();
        }
    }
}
