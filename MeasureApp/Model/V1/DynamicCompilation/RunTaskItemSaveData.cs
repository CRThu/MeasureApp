using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.DynamicCompilation
{
    public struct RunTaskItemSaveData
    {
        public string ParamVal;
        public string ResultVal;

        public RunTaskItemSaveData((string paramVal, string resultVal) value) : this()
        {
            ParamVal = value.paramVal;
            ResultVal = value.resultVal;
        }

        public void Deconstruct(out string paramVal, out string resultVal)
        {
            paramVal = ParamVal;
            resultVal = ResultVal;
        }
    }
}
