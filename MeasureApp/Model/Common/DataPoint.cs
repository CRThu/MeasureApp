using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Model.Common
{
    public struct DataPoint
    {
        public readonly double X { get; }
        public readonly double Y { get; }

        public DataPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
