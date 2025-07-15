using MeasureApp.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.FftAnalysis
{
    public class FftAnalysisReport : NotificationObjectBase
    {
        public string GroupName { get; set; }
        public string Name { get; set; }
        public dynamic Result1 { get; set; }
        public dynamic Result2 { get; set; }

        public FftAnalysisReport(string groupName, string name, dynamic result1 = null, dynamic result2 = null)
        {
            GroupName = groupName;
            Name = name;
            Result1 = result1;
            Result2 = result2;
        }
    }
}
