using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TaskMethodInfoAttribute : Attribute
    {
        public int Id { get; set; }
        public TaskMethodInfoAttribute(int id)
        {
            this.Id = id;
        }
    }
}
